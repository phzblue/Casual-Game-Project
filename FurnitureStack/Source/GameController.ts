import MouseController from "./MouseController";
import Util from "./Util";
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

    @property({
        type: cc.AudioClip
    })
    collideSFX: cc.AudioClip = null;

    @property({
        type: cc.AudioClip
    })
    starSFX: cc.AudioClip = null;

    @property({
        type: cc.AudioClip
    })
    splashSFX: cc.AudioClip = null;

    @property(cc.Node)
    blockContainer:cc.Node = null;

    @property(cc.Label)
    timerLabel:cc.Label = null;
    timerID:number;
    timerNumber:number = 90;

    @property(cc.Node)
    container:cc.Node = null;
    @property(cc.Node)
    hint1:cc.Node = null;
    @property(cc.Node)
    hint2:cc.Node = null;
    @property(cc.Node)
    hint3:cc.Node = null;

    @property(cc.Prefab)
    splashPrefab:cc.Prefab = null;
    @property(cc.Prefab)
    blockPrefab:cc.Prefab[] = [];
    @property(cc.Prefab)
    platformPrefab:cc.Prefab[] = [];
    
    @property(cc.Node)
    camOB:cc.Node = null;
    @property(cc.Node)
    gameOverUI:cc.Node = null;
    @property(cc.Node)
    previewSprite:cc.Node = null;
    @property(cc.Node)
    splash1:cc.Node = null;
    @property(cc.Node)
    splash2:cc.Node = null;

    @property(cc.Label)
    hiScoreLabel:cc.Label = null;
    @property(cc.Label)
    scoreLabel:cc.Label = null;

    @property(cc.Label)
    currentHeight:cc.Label = null;

    @property(cc.Node)
    platformContainer:cc.Node = null;
    platform:cc.Node = null;

    blockNum:number = 0;
    hiScore:number = 0;
    currentScore:number = 0;
    prevScore:number = 0;
    currentBlockHeight:number = 0;

    isGameOver:boolean = false;

    spawnedBlock:cc.Node[] = [];
    nextObjNum:number = -1;

    sdkApp:SDKFunctions = null;

    onLoad () {
        GameController.instance = this;
        this.sdkApp = this.getComponent(SDKFunctions);

        let physicsManager = cc.director.getPhysicsManager();
        physicsManager.enabled = true;
        //physicsManager.debugDrawFlags = 1;

        this.hiScore = cc.sys.localStorage.getItem("furniture_highscore") != null ? cc.sys.localStorage.getItem("furniture_highscore") : 0;   
    }

    start () {

        this.startTimer();
        this.platform = cc.instantiate(this.platformPrefab[Util.getRandom(this.platformPrefab.length)]);
        this.platform.setParent(this.platformContainer);

        this.predictNextObject();
        this.spawnBlock();
    }

    startTimer(){
        this.timerID = setInterval(()=>{
            this.timerNumber--;
            if(this.timerNumber <=0){
                this.displayGameOver();
                this.timerLabel.string = Util.secondToMinute(0);
                return;
            }
            this.timerLabel.string = Util.secondToMinute(this.timerNumber);
        },1000);
    }

    spawnWater(){
        let leftWater = cc.instantiate(this.splashPrefab);
        leftWater.setParent(this.node);

        let rightWater = cc.instantiate(this.splashPrefab);
        rightWater.getComponent(cc.Widget).isAlignLeft = false;
        rightWater.getComponent(cc.Widget).isAlignRight = true;
        rightWater.getComponent(cc.Widget).right = -150;
        rightWater.getComponent(cc.Widget).updateAlignment();
        rightWater.setParent(this.node);
    }

    restartGame(){
        this.blockNum = 0;
        this.currentScore = 0;
        this.currentBlockHeight = 0;

        this.timerNumber = 90;
        this.timerLabel.string = Util.secondToMinute(this.timerNumber);

        this.camOB.position = new cc.Vec2(0,0);

        this.currentHeight.string = "0";

        this.platformContainer.destroyAllChildren();
        this.platform = cc.instantiate(this.platformPrefab[Util.getRandom(this.platformPrefab.length)]);
        this.platform.setParent(this.platformContainer);

        this.spawnedBlock.forEach(element => {
            element.destroy();
        });
        
        for(let v of this.container.children){
            v.destroy();
        }

        this.predictNextObject();

        this.container.setContentSize(0,0);

        this.isGameOver = false;

        this.gameOverUI.active = false;

        this.hint1.active = true;

        this.spawnBlock();
    }

    playCollideSfx(){
        cc.audioEngine.setEffectsVolume(0.5);
        cc.audioEngine.playEffect(this.collideSFX,false);
    }

    displayGameOver(){   
        MouseController.instance.currentBlock.destroy();

        clearInterval(this.timerID);
        
        this.gameOverUI.position = this.camOB.position;
        this.isGameOver = true;
        this.gameOverUI.active = true;

        if(this.currentScore >  this.hiScore ){
            this.hiScore = this.currentScore;
            cc.sys.localStorage.setItem("furniture_highscore", this.hiScore);
        }
        this.updateGOScore();

        this.sdkApp.triggerGameOver();
    }

    displayWaterSplashes(pos:cc.Vec2){
        cc.audioEngine.play(this.splashSFX,false,1);
        if(pos.x > 0){
            // this.splash2.active = false;
            // this.splash2.active = true;

            // this.splash2.getComponent(cc.Animation).stop();
            this.splash2.getComponent(cc.Animation).play();
        }else{
            // this.splash1.active = false;
            // this.splash1.active = true;

            // this.splash1.getComponent(cc.Animation).stop();
            this.splash1.getComponent(cc.Animation).play();
        }
    }

    showStarAnim(pos){
        cc.audioEngine.setEffectsVolume(1);

        cc.audioEngine.playEffect(this.starSFX,false);

        // this.starAnim.setPosition(pos);
        // this.starAnim.getComponent(cc.Animation).play();
    }

    updateScore(){
        this.currentHeight.string = this.currentScore + "ft";
        this.sdkApp.sendCurrentScore(this.currentScore);
    }

    updateGOScore(){
        this.hiScoreLabel.string = "HIGHSCORE: " + this.hiScore + "ft";
        this.scoreLabel.string = "SCORE: " + this.currentScore + "ft";
    }

    predictNextObject(){
        this.nextObjNum = Util.getRandom(this.blockPrefab.length);
        this.previewSprite.getComponent(cc.Sprite).spriteFrame = this.blockPrefab[this.nextObjNum].data.getComponentInChildren(cc.Sprite).spriteFrame;
    }

    spawnBlock(){
        let block:cc.Node = cc.instantiate(this.blockPrefab[this.nextObjNum]);
        //block.getComponentInChildren(cc.Label).string = this.blockNum.toString();
        block.name = block.name+" "+this.blockNum;
        let spawnHeight = 0;

        if(Math.abs(this.currentBlockHeight) > 0){
            spawnHeight = this.currentBlockHeight + 750;
        }

        if(spawnHeight - this.camOB.position.y > 500){
            this.camOB.runAction(cc.moveBy(1,new cc.Vec2(0,200)))
        }

        block.position = new cc.Vec2(0, spawnHeight);
        block.setParent(this.blockContainer);
        cc.audioEngine.play(this.starSFX,false,1);

        this.spawnedBlock.push(block);
        this.blockNum++;

        MouseController.instance.currentBlock = block;

        this.predictNextObject();
    }

    moveCameraUp(){
        this.camOB.runAction(cc.moveBy(.5,new cc.Vec2(0,20)))
    }

    moveCameraDown(){
        if(!this.camOB.position.equals(cc.v2(0,0))){
            let diff = this.camOB.position.sub(cc.v2(0,20));
            if(diff.y < 0){
                this.camOB.stopAllActions();
                this.camOB.runAction(cc.moveTo(.5,new cc.Vec2(0,0)))
            }else{
                this.camOB.runAction(cc.moveBy(.5,new cc.Vec2(0,-20)))
            }
        }
    }

    displayQuitButton(){
        this.camOB.runAction(cc.moveTo(0,new cc.Vec2(0,0)))
        this.gameOverUI.getComponentInChildren(cc.Label).string = "Tap To Show Result Now!"
        this.gameOverUI.getComponent(cc.Button).enabled = true;
    }

    update(dt){
        if(!this.isGameOver){
            if(this.container.getContentSize().height > 10){
                this.currentBlockHeight = this.platform.position.y + this.container.getContentSize().height;
                //this.checker.position = new cc.Vec2(0,this.currentBlockHeight);
            }

            this.currentScore = Math.floor( this.container.getContentSize().height);

            if(this.prevScore != this.currentScore){
                this.prevScore = this.currentScore;
                this.updateScore();
            }
        }
    }

}
