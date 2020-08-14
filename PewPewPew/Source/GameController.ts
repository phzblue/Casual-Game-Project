// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import Util from "./Util";
import Globe from "./Globe";
import ScoreController from "./ScoreController";

const {ccclass, property} = cc._decorator;

@ccclass
export default class GameController extends cc.Component {

    static instance:GameController = null;

    @property(Globe)
    globeController:Globe = null;
    scoreController:ScoreController = null;

    @property(cc.Label)
    timerLabel:cc.Label = null;
    @property(cc.Node)
    gameOverUI:cc.Node = null;
    @property(cc.Node)
    hintUI:cc.Node = null;
    @property(cc.Node)
    alertNode:cc.Node = null;

    @property({
        type:cc.AudioClip
    })
    chickenSFX:cc.AudioClip = null;

    timerID:number;
    timer:number = 90;
    hintTimer:number = 0;

    isGameOver:boolean = false;

    onLoad () {
        if(GameController.instance == null){
            GameController.instance = this;
        }

        cc.director.getPhysicsManager().enabled = true;
        //cc.director.getPhysicsManager().debugDrawFlags = 1;
        cc.director.getCollisionManager().enabled = true;
        //cc.director.getCollisionManager().enabledDebugDraw = true;

        this.scoreController = this.getComponent(ScoreController);
    }

    start () {
        this.timerID = setInterval(()=>{
            this.hintTimer++;

            if(this.hintTimer==5){
                this.hintUI.active = false;
            }

            this.timer--;
            this.timerLabel.string = Util.secondToMinute(this.timer);
            if(this.timer <=0){
                this.isGameOver = true;
                this.endGame();
            }

            if(this.timer == 33 || this.timer == 63){
                cc.audioEngine.play(this.chickenSFX,false,1);
                this.alertNode.getComponent(cc.Animation).play();
            }

            if(this.timer == 30 || this.timer == 60){
                this.globeController.changeDirection();
            }
        },1000);
    }

    endGame(){
        // this.jumper.getComponent(Jumper).stopTween();
        clearInterval(this.timerID);
        this.gameOverUI.active = true;
    }

    abort(){
        this.gameOverUI.getComponent(cc.Button).enabled = true;
        this.gameOverUI.getComponentInChildren(cc.Label).string = "Tap Now To Show Result!";
    }

    // update (dt) {}
}
