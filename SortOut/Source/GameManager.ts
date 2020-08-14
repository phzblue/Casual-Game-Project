import BlockBase from "./BlockBase";
import BlockManager from "./BlockManager";
import Util from "./Util"
import AnimationMethod from "./AnimationMethod";
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

enum SwipeDirection{
    UP,DOWN,LEFT,RIGHT
}

@ccclass
export default class GameManager extends cc.Component {

    static instance:GameManager = null;

    @property(cc.Node)
    startUI:cc.Node = null;

    @property(cc.Prefab)
    blockPrefab: cc.Prefab = null;
    @property(cc.Node)
    parentRoom: cc.Node = null;
    @property(cc.Node)
    container: cc.Node = null;
    @property(cc.Node)
    gameoverUI:cc.Node = null;
    @property(cc.Node)
    titleImage:cc.Node = null;
    @property(cc.Node)
    helpNode:cc.Node = null;
    @property(cc.Node)
    hintMenu: cc.Node = null;

    @property(cc.Label)
    moveLabel:cc.Label = null;
    moveNum:number = 50;

    @property(cc.Label)
    pointLabel:cc.Label = null;
    pointNum:number = 0;
    @property(cc.Label)
    highscoreLabel:cc.Label = null;
    highscore:number = 0;

    @property(cc.Label)
    timerLabel:cc.Label = null;
    timerID:number = null;
    timerNum:number = 10;

    startDragPos: cc.Vec2 = new cc.Vec2(0,0);
    endDragPos: cc.Vec2 = new cc.Vec2(1,1);
    freePos: cc.Vec2 = new cc.Vec2(0,0);

    swipe:SwipeDirection = SwipeDirection.UP;

    isPlayable:boolean = false;
    firstHiddenBlock:boolean = true;

    cellNum:number = 6;
    @property(cc.Node)
    comboContainer:cc.Node = null;
    multiplier:number = 0;
    scoreAccumulation:number = 0;

    obstacleStartCountDown:boolean = false;
    obstacleCountDown:number = 10;

    blockManager:BlockManager = null;

    @property(cc.Node)
    scoreAnimContainer:cc.Node = null;
    @property(cc.Prefab)
    scorePrefab:cc.Prefab = null;

    @property({
        type: cc.AudioClip
    })
    slideSFX: cc.AudioClip = null;

    @property({
        type: cc.AudioClip
    })
    slideOutSFX: cc.AudioClip = null;

    @property({
        type: cc.AudioClip
    })
    gameOverSFX: cc.AudioClip = null;

    @property({
        type: cc.AudioClip
    })
    lightUpSFX: cc.AudioClip = null;

    sdkApp:SDKFunctions = null;

    onLoad(){
        if(GameManager.instance == null){
            GameManager.instance = this;
        }
        
        this.sdkApp = this.getComponent(SDKFunctions);
    }

    start () {

        this.blockManager = new BlockManager();
        this.blockManager.blockPrefab = this.blockPrefab;
        this.blockManager.setAudio(this.lightUpSFX);
        this.startNewGame();
        
        this.node.on(cc.Node.EventType.TOUCH_START,function(event:cc.Touch){
            this.closeTitle();
            this.startDragPos = event.getLocation();
        },this)

        this.node.on(cc.Node.EventType.TOUCH_END,function(event:cc.Touch){
            this.endDragPos = event.getLocation();
            this.calculateDragDistance();
        },this)

        let hs = cc.sys.localStorage.getItem("highscore");
        this.highscore = hs != null ? hs : 0;    
        this.updateHiScore();

        this.startTimer();
    }

    openHint(){
        this.hintMenu.active = true;
    }

    closeHint(){
        this.hintMenu.active = false;
    }

    closeTitle(){
        if(this.titleImage.active){
            this.titleImage.active = false;

            this.helpNode.active = false;
        }
    }

    startNewGame(){
        this.closeHint();
        this.clearGame();
        this.generatePuzzleBlock();
        this.generateFreeSpot();
        this.generateExit();
    }

    startTimer(){

        this.startUI.active = false;

        this.timerID = setInterval(()=>{

            this.timerNum--;
            this.timerLabel.string = Util.secondToMinute(this.timerNum);
            if(this.timerNum <=0){
                this.showGameOver();
                clearInterval(this.timerID);
            }
        },1000);

    }

    clearGame(){

        this.startUI.active = true;
        this.gameoverUI.active = false;

        this.timerNum = 90;
        this.timerLabel.string = Util.secondToMinute(this.timerNum);

        this.blockManager.resetGame();

        this.cellNum = 6;
        this.scoreAccumulation = 0;
        this.isPlayable = true;
        this.firstHiddenBlock = true;

        this.pointNum = 0;
        this.pointLabel.string = this.pointNum.toString();
        this.moveNum = 50;
        // this.moveLabel.string = this.moveNum.toString();

        this.multiplier = 0;

        this.parentRoom.removeAllChildren();
        this.obstacleStartCountDown = false;
        this.obstacleCountDown = 10;
    }

    updateHiScore(){
        this.highscoreLabel.string = this.highscore.toString();
    }

    updateMoveNum(){
        if(this.moveNum>0){
            this.moveNum--;
            this.moveLabel.string = this.moveNum.toString();

            if(this.obstacleStartCountDown){
                this.obstacleCountDown--;

                if(this.obstacleCountDown <= 0){
                    this.obstacleCountDown = 5;
                    this.blockManager.findObstacleBlock();
                    this.obstacleStartCountDown = false;
                }
            }
        }

        if(this.moveNum==0){
            this.isPlayable = false;
            this.gameoverUI.active = true;

            cc.audioEngine.playEffect(this.gameOverSFX, false);

            if(this.pointNum > this.highscore){
                cc.sys.localStorage.setItem("highscore", this.pointNum);
                this.highscore = this.pointNum;
                this.updateHiScore();   
            }

            this.sdkApp.triggerGameOver();
        }

        this.pointLabel.string = this.pointNum.toString();
    }

    showGameOver(){
        this.isPlayable = false;
            this.gameoverUI.active = true;

            cc.audioEngine.playEffect(this.gameOverSFX, false);

            if(this.pointNum > this.highscore){
                cc.sys.localStorage.setItem("highscore", this.pointNum);
                this.highscore = this.pointNum;
                this.updateHiScore();   
            }

            this.sdkApp.triggerGameOver();
    }

    calculateDragDistance(){
        if(this.isPlayable){
            let direction = this.endDragPos.sub(this.startDragPos);
            if(Math.abs(direction.x) > Math.abs(direction.y) ){
                if(Math.sign(direction.x) == 1){
                    //positive drag = swipe right
                    this.moveBlock(SwipeDirection.RIGHT);
                }else if(Math.sign(direction.x) == -1){
                    //negative drag = swipe left
                    this.moveBlock(SwipeDirection.LEFT);
                }
    
            }else if(Math.abs(direction.y) > Math.abs(direction.x)){
                if(Math.sign(direction.y) == 1){
                    //positive drag = swipe up
                    this.moveBlock(SwipeDirection.UP);
                }else if(Math.sign(direction.y) == -1){
                    //negative drag = swipe down
                    this.moveBlock(SwipeDirection.DOWN);
                }
            }
        }
    }

    addScore(score:number){
        let newScore = cc.instantiate(this.scorePrefab);
        newScore.getComponent(cc.Label).string = "x"+score;
        this.scoreAnimContainer.addChild(newScore);
        cc.tween(newScore).parallel(
            cc.fadeOut(1),cc.moveBy(1,cc.v2(0,80))
        )
        .call(()=>{newScore.destroy()})
        .start();
    }

    moveBlock(direction:SwipeDirection){
        let currentCoor = this.vec2ToCoordinate(this.freePos,direction);

        if(this.checkForExitScore(direction)){
            this.multiplier++;

            if(this.pointNum > 40){
                this.scoreAccumulation += 1 * this.multiplier;
            }

            this.pointNum += 1 * this.multiplier;

            this.comboContainer.getComponentInChildren(cc.Label).string = "x"+this.multiplier;
            this.comboContainer.active = this.multiplier>1;

            this.addScore(1 * this.multiplier);
            this.pointLabel.string = this.pointNum.toString();

            this.sdkApp.sendCurrentScore(this.pointNum);

            //this.moveNum += 5;

            //this.updateMoveNum();

            //this.convertColorBlockToObstacle();
        }else{

            if(this.blockManager.colorBlockContainer.has(currentCoor) &&
            !this.blockManager.colorBlockContainer.get(currentCoor).getComponent(BlockBase).isExit &&
            !this.blockManager.colorBlockContainer.get(currentCoor).getComponent(BlockBase).isObstacleBlock){

                let movingBlock = this.blockManager.colorBlockContainer.get(currentCoor).getComponent(BlockBase);
                let currentFreeBlock = this.blockManager.colorBlockContainer.get(this.freePos.x+","+this.freePos.y).getComponent(BlockBase);
                currentFreeBlock.isHiddenBlock = movingBlock.isHiddenBlock;

                movingBlock.getComponent(BlockBase).playShinyAnim(true);
                movingBlock.playMoveAnimation(false, direction, ()=>{
                    currentFreeBlock.setColor(movingBlock.color, movingBlock.isHiddenBlock);
                    movingBlock.resetBlock();

                    this.blockManager.checkNearExit(Util.vectorToString(this.freePos),currentFreeBlock.color, (a) => {
                        if(a){
                            if(currentFreeBlock.isHiddenBlock){
                                this.isPlayable = false;
                                currentFreeBlock.setUnhiddenAnimation(()=>{
                                    this.isPlayable = true;
                                    cc.audioEngine.playEffect(this.lightUpSFX,false);
                                    currentFreeBlock.getComponent(BlockBase).playShinyAnim(false);
                                });
                            }else{
                                cc.audioEngine.playEffect(this.lightUpSFX,false);
                                currentFreeBlock.getComponent(BlockBase).playShinyAnim(false);
                            }

                            
                        }else{
                            this.isPlayable = true;
                        }

                        this.freePos = Util.stringToVector(currentCoor);   
                        this.multiplier = 0;  

                        this.comboContainer.getComponentInChildren(cc.Label).string = "x"+this.multiplier;
                        this.comboContainer.active = this.multiplier>1;

                        this.pointLabel.string = this.pointNum.toString();

                        //this.updateMoveNum(); 
                    });

                    //this.isPlayable = true;

                    // this.freePos = Util.stringToVector(currentCoor);   
                    // this.multiplier = 0;  

                    // this.updateMoveNum();                    
                });
            }
        }   
    }

    generatePuzzleBlock(){
        this.blockManager.generatePuzzleBlock(this.parentRoom,this.cellNum);
    }

    generateFreeSpot(){
        this.freePos = this.blockManager.generateFreeBlock();
    }

    generateExit(){
        for(let i = 0; i<4; i++){
            this.blockManager.generateExit(this.cellNum);
        }

        this.blockManager.checkAllExit();
    }

    spawnNewBlock(newFreeSpot:string){

        this.blockManager.generateNewColorBlock(this.freePos,newFreeSpot,(newVar) =>{
            
            this.freePos = newVar;
            this.isPlayable = true;

            if(this.pointNum >= 10){
                this.blockManager.addNewExit(this.cellNum);
                this.blockManager.checkAllExit();
            }

            this.addHiddenProperty();

        });
    }


    addHiddenProperty(){
        if(this.pointNum >= 40){
            if(this.firstHiddenBlock){
                this.firstHiddenBlock = false;
                this.blockManager.generateHiddenBlock(Util.vectorToString(this.freePos));
            }else if(this.scoreAccumulation >= 5){
                this.scoreAccumulation = 0;
                this.blockManager.generateHiddenBlock(Util.vectorToString(this.freePos));
            }
        }
    }

    convertColorBlockToObstacle(){
        this.blockManager.convertColorBlockToObstacle(this.pointNum, Util.vectorToString(this.freePos),
        ()=>this.obstacleStartCountDown = true);
    }

    checkForExitScore(direction:SwipeDirection){
        let swipeCoor;
        for(let exitBlock of this.blockManager.exitBlockContainer.entries()){
            swipeCoor = this.vec2ToCoordinate(Util.stringToVector(exitBlock[0]),direction);

            //exitBlock[0] this is the coordinate
            if(exitBlock[1].getComponent(BlockBase).isActive &&
            this.blockManager.colorBlockContainer.has(swipeCoor))
            {
                let isScore = exitBlock[1].getComponent(BlockBase).exitColor.indexOf(this.blockManager.colorBlockContainer.get(swipeCoor).getComponent(BlockBase).color);
                if(isScore != -1){

                    this.container.getComponent(AnimationMethod).shake(direction);

                    this.isPlayable = false;

                    this.blockManager.colorBlockContainer.get(swipeCoor).getComponent(BlockBase).playShinyAnim(true);
                    this.blockManager.colorBlockContainer.get(swipeCoor).getComponent(BlockBase)
                    .playMoveAnimation(true, direction, ()=>{
                        this.blockManager.colorBlockContainer.get(swipeCoor).getComponent(BlockBase).resetBlock();
                        this.spawnNewBlock(swipeCoor);

                        //this.convertColorBlockToObstacle();
                    });

                    if(this.pointNum >= 20){
                        
                        this.blockManager.respawnExitBlock(exitBlock[1].getComponent(BlockBase),isScore,
                        exitBlock[1].getComponent(BlockBase).exitColor[isScore]);
                    }
                    
                    cc.audioEngine.playEffect(this.slideOutSFX, false);
                    
                    return true;
                }                
            }
        }
        cc.audioEngine.playEffect(this.slideSFX, false);

        return false;
    }

    vec2ToCoordinate(vec:cc.Vec2, direction:SwipeDirection){
        let endResult:cc.Vec2;
        switch (direction) {
            case SwipeDirection.UP:
                endResult = vec.add(new cc.Vec2(1,0));
                break;
            case SwipeDirection.DOWN:
                endResult = vec.sub(new cc.Vec2(1,0));
                break;
            case SwipeDirection.LEFT:
                endResult = vec.add(new cc.Vec2(0,1));
                break;
            case SwipeDirection.RIGHT:
                endResult = vec.sub(new cc.Vec2(0,1));
                break;
        }

        return endResult.x+","+endResult.y;
    }

    showAbortButton(){
        this.gameoverUI.getComponent(cc.Button).enabled = true;
        this.gameoverUI.getComponentInChildren(cc.Label).string = "Tap To Show Result!"
    }
}
