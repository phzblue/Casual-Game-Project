// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import GameController from "./GameController";

const {ccclass, property} = cc._decorator;

@ccclass
export default class ScoreController extends cc.Component {

    @property(cc.Prefab)
    scorePrefab:cc.Prefab = null;
    @property(cc.Node)
    bowlContainer:cc.Node = null
    @property(cc.Label)
    scoreLabel:cc.Label = null;

    currentScore:number = 0;

    normalScore(odenObj:cc.Node){
        this.createScorePrefab(1,odenObj);
        this.currentScore++;

        this.updateLabel();

        GameController.instance.sdkApp.sendCurrentScore(this.currentScore);

        cc.tween(odenObj).delay(2).call(()=>{
            odenObj.destroy();
        })
    }

    comboScore(){
        let comboNum = GameController.instance.stick.countCombo();
        let finalScore = 0;
        switch(comboNum){
            case 1:
                finalScore = 9;
                break;
            case 2:
                finalScore = 6;
                break;
            case 3:
                finalScore = 3;
                break;
        }
        
        this.currentScore += finalScore;
        GameController.instance.sdkApp.sendCurrentScore(this.currentScore);

        this.createScorePrefab(finalScore, this.bowlContainer);

        this.updateLabel();
    }

    createScorePrefab(score, obj:cc.Node){
        let scoreObj = cc.instantiate(this.scorePrefab);
        scoreObj.getComponent(cc.Label).string = "+"+score;

        scoreObj.setParent(obj);
        
        cc.tween(scoreObj)
        .parallel(
            cc.moveBy(0.5, cc.v2(0,100)),
            cc.fadeOut(0.5)
        )
        .start();
    }

    updateLabel(){
        this.scoreLabel.string = ""+this.currentScore;
    }

    reset(){
        this.currentScore = 0;
        this.updateLabel();
    }


}
