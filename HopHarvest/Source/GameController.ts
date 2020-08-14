import BunniController from "./BunniController";
import Util from "./Util";
import SpawnController from "./SpawnController";
import ScoreKeeper from "./ScoreKeeper";
import HarvestController from "./HarvestController";
import SDKFunctions from "./SDKFunction";

// Learn TypeScript:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/typescript.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/reference/attributes.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/life-cycle-callbacks.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/life-cycle-callbacks.html

const {ccclass, property} = cc._decorator;

@ccclass
export default class GameController extends cc.Component {

    static instance:GameController = null;

    sdkApp:SDKFunctions = null;
    
    @property(cc.Node)
    bunni:cc.Node = null;
    @property(cc.Node)
    spawnController:cc.Node = null;

    scoreKeeper:ScoreKeeper = null;
    harvestController:HarvestController = null;
    bunnicontroller:BunniController = null;

    @property(cc.Node)
    gameoverUI:cc.Node = null;
    @property(cc.Node)
    goScoreText:cc.Node = null;
    @property(cc.Node)
    goHiScoreText:cc.Node = null;
    @property(cc.Node)
    startUI:cc.Node = null;

    @property(cc.Node)
    scoreText:cc.Node = null;
    @property(cc.Node)
    timerText:cc.Node = null;
    
    currentScore:number = 0;
    timer:number = 90;
    hiScore:number = 0;
    speedTimer:number = 0;

    timerID:number = 0;

    isGameOver:boolean = true;

    // LIFE-CYCLE CALLBACKS:

    onLoad () {
        GameController.instance = this;

        this.sdkApp = this.getComponent(SDKFunctions);

        cc.director.getCollisionManager().enabled = true;
        
        this.bunnicontroller = this.bunni.getComponentInChildren(BunniController);
        this.scoreKeeper = this.node.getComponent(ScoreKeeper);
        this.harvestController = this.node.getComponent(HarvestController);
        this.hiScore = cc.sys.localStorage.getItem("bunni_hiscore") != null ? cc.sys.localStorage.getItem("bunni_hiscore") : 0;
    }

    start () {
        this.resetGame();
        this.startGame();
    }

    startGame(){
        this.startUI.active = false;
        this.timerID = setInterval(()=>{
            this.speedTimer++;

            if(this.speedTimer == 2){
                this.bunnicontroller.increaseSpeed();
                this.speedTimer = 0;
            }

            this.timer--;
            this.timerText.getComponent(cc.Label).string = this.secondToMinute(this.timer);
            if(this.timer <=0){
                this.displayGameOver();
            }

        },1000);

        this.spawnController.getComponent(SpawnController).spawnFirst7Veg();

    }

    secondToMinute(number:number){
        let min, sec;

        min = Math.floor(number / 60);
        sec = ("0" + Math.floor(number>=60 ? number-60 : number)).slice(-2)

        return min+":"+sec;
    }

    runAnimation(clipName:string){
        let anim = this.bunni.getComponent(cc.Animation);
        let animState = anim.getAnimationState(clipName);
        anim.on("finished", function(event) {
            if(event.currentTarget == animState) {
                console.log(clipName + "has finished");
            }
        });
        anim.play(clipName);
    }

    displayGameOver(){
        clearInterval(this.timerID);

        this.isGameOver = true;

        this.gameoverUI.active = true;

        if(this.currentScore > this.hiScore){
            cc.sys.localStorage.setItem("bunni_hiscore", this.currentScore);
            this.hiScore = this.currentScore;
        }

        this.sdkApp.triggerGameOver();

        this.goScoreText.getComponent(cc.Label).string = "SCORE: " + this.currentScore;
        this.goHiScoreText.getComponent(cc.Label).string = "HISCORE: " + this.hiScore;
    }

    displayAbort(){
        this.gameoverUI.getComponent(cc.Button).enabled = true;
        this.gameoverUI.getComponentInChildren(cc.Label).string = "Tap To Show Result Now!";
    }

    resetGame(){
        this.gameoverUI.active = false;
        this.startUI.active = true;
        this.currentScore = 0;
        this.timer = 90;
        this.speedTimer = 0;

        this.scoreText.getComponent(cc.Label).string = "0";
        this.timerText.getComponent(cc.Label).string = this.secondToMinute(this.timer);

        this.spawnController.getComponent(SpawnController).reset();
        this.scoreKeeper.reset();
        this.bunnicontroller.reset();

        this.isGameOver = false;
    }

    // update (dt) {}
}
