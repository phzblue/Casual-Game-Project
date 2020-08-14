// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import StickClass from "./StickClass";
import SpawnController from "./SpawnController";
import Util from "./Util";
import BowlClass from "./BowlClass";
import ScoreController from "./ScoreController";
import SDKFunctions from "./SDKFunction";

const {ccclass, property} = cc._decorator;

@ccclass
export default class GameController extends cc.Component {

    static instance:GameController = null;
    isGameOver:boolean = false;

    static spawnInterval:number = 3; 

    @property(cc.Node)
    titleUI:cc.Node = null;
    @property(cc.Node)
    gameOverUI:cc.Node = null;
    @property(cc.Label)
    timerLabel:cc.Label = null;
    @property(cc.Label)
    gameoverHiScore = null;
    @property(cc.Label)
    gameoverScore = null;

    @property(StickClass)
    stick:StickClass = null;
    @property(SpawnController)
    spawnController:SpawnController = null;
    @property(BowlClass)
    bowlObj:BowlClass = null;

    scoreController:ScoreController = null;
    hiScore:number = 0;
    timer:number = 90;
    timerID:number;
    sdkApp:SDKFunctions = null;

    onLoad(){
        GameController.instance = this;
        this.scoreController = this.node.getComponent(ScoreController);

        this.sdkApp = this.getComponent(SDKFunctions);

        cc.director.getPhysicsManager().enabled = true;
        cc.director.getCollisionManager().enabled = true;

        this.hiScore = cc.sys.localStorage.getItem("oden_hiscore") != null ? cc.sys.localStorage.getItem("oden_hiscore") : 0;
    }

    start () {
        this.resetGame();
        this.startGame();
    }

    startGame(){
        cc.director.getPhysicsManager().enabled = true;

        this.titleUI.active = false;
        this.spawnController.startSpawn();

        this.timerID = setInterval(()=>{
            this.timer--;
            this.timerLabel.string = Util.secondToMinute(this.timer);
            if(this.timer <=0){
                this.endGame();
            }
        },1000);
    }

    endGame(){
        cc.director.getPhysicsManager().enabled = false;
        clearInterval(this.timerID);
        this.isGameOver = true;

        if(this.scoreController.currentScore > this.hiScore){
            cc.sys.localStorage.setItem("oden_hiscore",this.scoreController.currentScore);
            this.hiScore = this.scoreController.currentScore;
        }

        this.gameoverHiScore.string = "High Score: " + this.hiScore;
        this.gameoverScore.string = "Score: " + this.scoreController.currentScore;
        this.gameOverUI.active = true;

        this.sdkApp.triggerGameOver();
    }

    displayAbortButton(){
        this.gameOverUI.getComponent(cc.Button).enabled = true;
        this.gameOverUI.getComponentInChildren(cc.Label).string = "Tap To Show Result Now!";
    }

    resetGame(){
        this.scoreController.reset();
        this.spawnController.reset();
        this.stick.reset();
        this.bowlObj.reset();

        this.titleUI.active = true;
        this.gameOverUI.active = false;
        this.isGameOver = false;

        this.timer = 90;
        this.timerLabel.string = Util.secondToMinute(this.timer);
    }

    // update (dt) {}
}
