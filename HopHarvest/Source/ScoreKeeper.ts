import GameController from "./GameController";

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
export default class ScoreKeeper extends cc.Component {

    currentVeggieType:number = null;

    multiplierPoint:number = 0;

    @property(cc.Prefab)
    scorePrefab:cc.Prefab = null;

    @property(cc.Node)
    scoreContainer:cc.Node = null;

    addScore(veggieType:number){
        if(this.currentVeggieType == null || this.currentVeggieType != veggieType ){
            this.multiplierPoint = 1;
            this.currentVeggieType = veggieType
        }else if(this.currentVeggieType == veggieType){
            this.multiplierPoint++;
        }

        let finalScore:number = 0;
        if(this.multiplierPoint == 1){
            finalScore = 1;
            GameController.instance.currentScore++;
        }else{
            finalScore = this.multiplierPoint * this.multiplierPoint;
            GameController.instance.currentScore += finalScore;
        }
        GameController.instance.scoreText.getComponent(cc.Label).string = GameController.instance.currentScore.toString();

        if(this.multiplierPoint > 1){
            let scoreDisplay = cc.instantiate(this.scorePrefab);
            scoreDisplay.getComponent(cc.Label).string = "COMBO X" + this.multiplierPoint;
    
            this.scoreContainer.addChild(scoreDisplay);

            cc.tween(scoreDisplay).sequence(cc.delayTime(0.5),cc.fadeOut(0.5),cc.callFunc(()=>{ scoreDisplay.destroy() })).start();
        }

        GameController.instance.sdkApp.sendCurrentScore(GameController.instance.currentScore);

        return finalScore;
    }

    reset(){
        this.currentVeggieType = null;
        this.multiplierPoint = 0;
    }

    // update (dt) {}
}
