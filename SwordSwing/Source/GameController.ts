// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import Util from "./Util";
import ScoreController from "./ScoreController";

const {ccclass, property} = cc._decorator;

@ccclass
export default class GameController extends cc.Component {

    static instance:GameController = null;

    spawnTimerID:number;
    timerID:number;
    timer:number = 90;
    isGameOver:boolean = false;
    scoreController:ScoreController = null;

    @property({
        type:cc.AudioClip
    })
    sfx:cc.AudioClip = null;

    @property(cc.Prefab)
    enermyPrefab:cc.Prefab = null;
    @property(cc.Node)
    slayer:cc.Node = null;

    @property(cc.Label)
    timerLabel:cc.Label = null;
    @property(cc.Node)
    gameOverUI:cc.Node = null;
    @property(cc.Node)
    grassPatch:cc.Node = null;

    onLoad () {
        if(GameController.instance == null){
            GameController.instance = this;
        }

        this.scoreController = this.getComponent(ScoreController);

        cc.director.getPhysicsManager().enabled = true;
        cc.director.getCollisionManager().enabled = true;
    }

    start () {
        this.timerID = setInterval(()=>{

            this.timer--;
            this.timerLabel.string = Util.secondToMinute(this.timer);
            if(this.timer <=0){
                this.isGameOver = true;
                this.endGame();
            }
            this.spawnEnermy();

        },700);
    }

    spawnEnermy(){
        if(!this.isGameOver){
            let enermy:cc.Node = cc.instantiate(this.enermyPrefab);
            
            enermy.setParent(this.grassPatch);
            enermy.setPosition(
                Util.getRandom(this.grassPatch.width/2-50, -this.grassPatch.width/2+50),
                Util.getRandom(this.grassPatch.height/2-50,-this.grassPatch.height/2+50)
            )
        }
    }

    endGame(){
        cc.director.getPhysicsManager().enabled = false;
        cc.director.getCollisionManager().enabled = false;
        clearInterval(this.timerID);
        this.gameOverUI.active = true;
    }

    abort(){
        this.gameOverUI.getComponent(cc.Button).enabled = true;
        this.gameOverUI.getComponentInChildren(cc.Label).string = "Tap Now To Show Result!";
    }

    // update (dt) {}
}
