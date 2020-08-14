import CardGenerator from "./CardGenerator";
import CardAnim from "./CardAnim";
import EmojiGenerator from "./EmojiGenerator";
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
    static scoreVariable:number = 5;

    cardGenerator:CardGenerator = null;

    firstSwipe:boolean = true;

    isGameOver:boolean = false;
    currentScore:number = 0;
    timer:number = 60;
    hiScore:number = 0;
    timerID:number = 0;
    cardNum:number = 0;

    currentNumber:number = -1;
    prevNumber:number = null;

    @property({
        type:cc.AudioClip
    })
    increaseScoreSfx:cc.AudioClip = null;
    @property({
        type:cc.AudioClip
    })
    decreaseScoreSfx:cc.AudioClip = null;
    @property({
        type:cc.AudioClip
    })
    timerSfx:cc.AudioClip = null;
    @property({
        type:cc.AudioClip
    })
    swipeSfx:cc.AudioClip = null;


    @property(cc.Prefab)
    scoreIncrePrefab:cc.Prefab = null;

    @property(cc.Node)
    startUI:cc.Node = null;
    @property(cc.Node)
    gameoverUI:cc.Node = null;
    @property(cc.Node)
    timerText:cc.Node = null;
    @property(cc.Node)
    scoreText:cc.Node = null;
    @property(cc.Node)
    hintTextContainer:cc.Node = null;
    @property(cc.Node)
    cardNumText:cc.Node = null;
    @property(cc.Node)
    goScoreText:cc.Node = null;
    @property(cc.Node)
    goHiScoreText:cc.Node = null;
    @property(cc.Node)
    scoreIncrementContainer:cc.Node = null;
    @property(cc.Node)
    cardBack:cc.Node = null;
    @property(cc.Node)
    leftArrow:cc.Node = null;
    @property(cc.Node)
    rightArrow:cc.Node = null;

    @property(cc.Node)
    pre:cc.Node = null;

    sdkApp:SDKFunctions = null;

    // LIFE-CYCLE CALLBACKS:

    onLoad () {
        GameController.instance = this;
        this.sdkApp = this.getComponent(SDKFunctions);

        this.cardGenerator = this.node.getComponent(CardGenerator);
        this.hiScore = cc.sys.localStorage.getItem("number_hiscore") != null ? cc.sys.localStorage.getItem("number_hiscore") : 0;
    }

    start () {
        this.resetGame();
        this.startGame();
    }

    startGame(){

        this.startUI.active = false;
        this.timerID = setInterval(()=>{
            if(this.timer <= 5){
                cc.audioEngine.playEffect(this.timerSfx,false);
            }

            this.timer--;
            this.timerText.getComponent(cc.Label).string = this.secondToMinute(this.timer);
            if(this.timer <=0){
                this.displayGameOver();
            }
        },1000);

        this.currentNumber = this.cardGenerator.generateCard(true);
    }

    secondToMinute(number:number){
        let min, sec;

        min = Math.floor(number / 60);
        sec = ("0" + Math.floor(number>=60 ? number-60 : number)).slice(-2)

        return min+":"+sec;
    }

    resetGame() {
        this.gameoverUI.active = false;
        this.startUI.active = true;
        this.currentScore = 0;
        this.timer = 60;
        this.firstSwipe = true;
        this.prevNumber = null;
        this.currentNumber = -1;
        this.cardNum = 0;

        if(this.cardBack.childrenCount>0){
            this.cardBack.children[0].destroy();
        }

        this.hintTextContainer.children[1].active = false;
        this.hintTextContainer.children[0].active = true;

        this.timerText.getComponent(cc.Label).string = this.secondToMinute(this.timer);
        this.scoreText.getComponent(cc.Label).string = "0";
        this.pre.getComponent(cc.Label).string = "0";
        this.cardNumText.getComponent(cc.Label).string = "0";

        this.isGameOver = false;
    }

    displayGameOver(){
        clearInterval(this.timerID);

        this.isGameOver = true;

        this.gameoverUI.active = true;

        if(this.currentScore > this.hiScore){
            cc.sys.localStorage.setItem("number_hiscore", this.currentScore);
            this.hiScore = this.currentScore;
        }

        this.goScoreText.getComponent(cc.Label).string = "SCORE: " + this.currentScore;
        this.goHiScoreText.getComponent(cc.Label).string = "HISCORE: " + this.hiScore;

        this.sdkApp.triggerGameOver();
    }

    displayAbortGame(){
        this.gameoverUI.getComponent(cc.Button).enabled = true;
        this.gameoverUI.getComponentInChildren(cc.Label).string = "Tap To Show Result Now!";

    }

    swipe(value:number){
        if(value > 0){
            this.flipCardAway(1);
        }else if(value < 0){
            this.flipCardAway(-1);
        }

        if(this.firstSwipe){
            this.hintTextContainer.children[0].active = false;
            this.hintTextContainer.children[1].active = true;
            this.firstSwipe = false;

            this.leftArrow.active = true;
            this.rightArrow.active = true;
        }
    }

    flipCardAway(direction){
        let childCard = this.cardBack.children[0];

        childCard.runAction(cc.sequence(cc.moveBy(0.2, 1000*direction, 0), cc.callFunc(()=>{
            childCard.parent = null;
            this.calculateScore(direction);

            this.currentNumber = this.cardGenerator.generateCard();

            cc.audioEngine.playEffect(this.swipeSfx,false);

        }), cc.delayTime(0.5), cc.callFunc(()=>{
            console.log("destory? " + childCard.destroy());
        })));
    }

    moveCard(direction){
        //if positive , card rotate left
        //if negative , card rotate right
        if(direction.x < 0){
            this.cardBack.children[0].getComponentInChildren(CardAnim).shiftCardLeft()
        }else if(direction.x > 0){
            this.cardBack.children[0].getComponentInChildren(CardAnim).shiftCardRight();
        }
    }

    calculateScore(direction){

        let isWin:boolean;

        if(this.prevNumber != null){
            if(direction == 1){
                //is current > prev?
                isWin = this.currentNumber>=this.prevNumber;
                this.score(isWin);
            }else{
                //is pre > current?
                isWin = this.prevNumber>=this.currentNumber;
                this.score(isWin);
            }
            this.node.getComponent(EmojiGenerator).spawnEmoji(direction,isWin);
            this.prevNumber = this.currentNumber;
        }
        
        this.prevNumber = this.currentNumber;
        this.pre.getComponent(cc.Label).string = this.prevNumber.toString();
    }

    score(scored){
        let scoreAnim = cc.instantiate(this.scoreIncrePrefab);

        if(scored){
            this.currentScore += GameController.scoreVariable;
            scoreAnim.getComponent(cc.Label).string = "+"+ GameController.scoreVariable;
            scoreAnim.color = cc.Color.GREEN;
            cc.audioEngine.playEffect(this.increaseScoreSfx,false);
        }else{
            this.currentScore -= GameController.scoreVariable;
            if(this.currentScore < 0){
                this.currentScore = 0;
            }
            scoreAnim.getComponent(cc.Label).string = "-"+GameController.scoreVariable;
            scoreAnim.color = cc.Color.RED;

            cc.audioEngine.playEffect(this.decreaseScoreSfx,false);

        }

        this.sdkApp.sendCurrentScore(this.currentScore);
        scoreAnim.setParent(this.scoreIncrementContainer);

        scoreAnim.runAction(cc.sequence(cc.spawn(cc.moveBy(.5,0,80),cc.fadeOut(.5)),cc.callFunc(()=>{
            scoreAnim.destroy();
        })));

        this.scoreText.getComponent(cc.Label).string = this.currentScore.toString();

    }

    cheat(){
        this.pre.active = true;
    }

}
