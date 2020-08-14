import Util from "./Util";
import ScoreKeeper from "./ScoreKeeper"
import ObjectGenerator from "./ObjectGenerator";
import PlayerController from "./PlayerController";
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

    @property(cc.Node)
    canvas:cc.Node = null;

    @property(cc.Node)
    startUI:cc.Node = null;
    @property(cc.Label)
    waitingMsg:cc.Label = null;

    @property(cc.Node)
    hintContainer:cc.Node = null;

    @property(cc.Node)
    gameOverUI:cc.Node = null;
    @property(cc.Label)
    gameOverHighScore = null;
    @property(cc.Label)
    gameOverScore = null;

    @property(cc.Label)
    timerText:cc.Label = null;

    @property(cc.Node)
    playObject:cc.Node = null;

    @property(ScoreKeeper)
    scoreKeeper:ScoreKeeper = null;

    @property(ObjectGenerator)
    objectGenerator:ObjectGenerator = null;

    @property(cc.Node)
    fishContainer:cc.Node = null;

    @property(cc.SpriteFrame)
    fish1:cc.SpriteFrame = null;
    @property(cc.SpriteFrame)
    fish2:cc.SpriteFrame = null;

    @property(cc.Prefab)
    rockPrefabs:cc.Prefab[] = [];

    @property(cc.Node)
    rockContainer:cc.Node = null;

    hiScore:number = 0;
    timer:number = 90;
    timerID:number;
    rockTimerID:number;
    fishTimerID:number;

    isGameOver:boolean = true;
    
    sdkFunctions:SDKFunctions = null;

    onLoad () {

        GameController.instance = this;
        this.sdkFunctions = this.getComponent(SDKFunctions);
        
        cc.director.getPhysicsManager().enabled = true;
        cc.director.getCollisionManager().enabled = true;

        this.hiScore = cc.sys.localStorage.getItem("epicFall_hiscore") != null ? cc.sys.localStorage.getItem("epicFall_hiscore") : 0;
    }

    start () {
        this.resetGame();
        this.startGame()
    }

    startGame(){
        this.startUI.active = false;
        this.isGameOver = false;

        PlayerController.instance.startDiving();

        cc.tween(this.hintContainer).delay(0.5).then(cc.fadeIn(0.5)).delay(7).then(cc.fadeOut(0.5)).start();

        this.timerID = setInterval(()=>{

            this.timer--;
            this.timerText.string = Util.secondToMinute(this.timer);
            if(this.timer <=0 && !this.isGameOver){
                this.displayGameOver();
            }
        },1000);

        this.fishTimerID = setInterval(()=>{
            this.generateFishAnimation();
        },5000);

        this.rockTimerID = setInterval(()=>{
            this.generateRocks();
        },3500);

        this.objectGenerator.startGenerating();
    }

    generateRocks(){
        let rockPos = Util.getRandom(this.rockContainer.getContentSize().width-200, -this.rockContainer.getContentSize().width+20);
        let rock = cc.instantiate(this.rockPrefabs[Util.getRandom(this.rockPrefabs.length)])
        
        cc.tween(this.rockContainer)
        .call(()=>{
            this.newRock();
        }).start();
    }

    newRock(){
        let rockPos = Util.getRandom(this.rockContainer.getContentSize().width, -this.rockContainer.getContentSize().width);
        let rock = cc.instantiate(this.rockPrefabs[Util.getRandom(this.rockPrefabs.length)])
        this.rockContainer.addChild(rock)
        rock.setPosition(rockPos, 0);
    }

    generateFishAnimation(){
        let fishNode:cc.Node = new cc.Node();
        fishNode.addComponent(cc.Sprite).spriteFrame = this.fish1;

        let fishNode2:cc.Node = new cc.Node();
        fishNode2.addComponent(cc.Sprite).spriteFrame = this.fish2;

        this.fishContainer.addChild(fishNode);
        this.fishContainer.addChild(fishNode2);

        fishNode2.setPosition(-800,Util.getRandom(450,-800));
        fishNode.setPosition(-800,Util.getRandom(450,-800));

        cc.tween(fishNode).by(Util.getRandomFloat(2,1),{position:cc.v2(1500,0)}).call(()=>{fishNode.destroy()}).start();
        cc.tween(fishNode2).by(Util.getRandomFloat(3,1.5),{position:cc.v2(1500,0)}).call(()=>{fishNode2.destroy()}).start();

    }

    resetGame(){
        cc.Camera.main.node.setPosition(0,0);
        this.playObject.setPosition(0,55);

        this.scoreKeeper.reset();
        this.objectGenerator.reset();

        //this.startUI.active = true;
        this.gameOverUI.active = false;

        this.timer = 90;
        this.timerText.string = Util.secondToMinute(this.timer);
    }

    displayGameOver(){
        cc.director.getPhysicsManager().enabled = false;

        PlayerController.instance.stopAnim();

        clearInterval(this.timerID);
        clearInterval(this.fishTimerID);
        clearInterval(this.rockTimerID);
        this.isGameOver = true;

        if(this.scoreKeeper.currentScore > this.hiScore){
            cc.sys.localStorage.setItem("epicFall_hiscore",this.scoreKeeper.currentScore);
            this.hiScore = this.scoreKeeper.currentScore;
        }

        this.gameOverHighScore.string = "High Score: " + this.hiScore;
        this.gameOverScore.string = "Score: " + this.scoreKeeper.currentScore;
        this.gameOverUI.active = true;

        this.sdkFunctions.triggerGameOver();
    }

    displayQuitButton(){
        this.gameOverUI.getComponent(cc.Button).enabled = true;
        this.gameOverUI.getComponentInChildren(cc.Label).string = "Tap To Show Result!";
    }
}
