// Learn TypeScript:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/typescript.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/reference/attributes.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/life-cycle-callbacks.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/life-cycle-callbacks.html

import GameController from "./GameController";

const {ccclass, property} = cc._decorator;

@ccclass
export default class ScoreKeeper extends cc.Component {

    currentScore:number = 0;
    currentCombo:number = 0;

    @property(cc.Prefab)
    scorePrefab:cc.Prefab = null;

    @property(cc.Label)
    scoreText:cc.Label = null;
    @property(cc.Label)
    comboText:cc.Label = null;

    @property(cc.Node)
    playerObj:cc.Node = null;

    increaseScore(){
        this.currentCombo++;

        let finalScore:number = 0;
        if(this.currentCombo == 1){
            finalScore = 1;
            this.currentScore++;
        }else{
            finalScore = this.currentCombo;
            this.currentScore += finalScore;
            this.comboText.node.active = true;
        }

        let scorePoint = cc.instantiate(this.scorePrefab);
        scorePoint.getComponent(cc.Label).string = "+"+finalScore;

        this.playerObj.addChild(scorePoint);
        cc.tween(scorePoint).parallel(cc.fadeOut(1),cc.moveBy(1,cc.v2(0,80))).call(()=>{scorePoint.destroy()}).start();

        this.scoreText.string = this.currentScore.toString();
        this.updateText();

        GameController.instance.sdkFunctions.sendCurrentScore(this.currentScore);
    }

    updateText(){
        this.comboText.node.active = !(this.currentCombo == 0);

        this.comboText.string = "Combo x" + this.currentCombo;
    }

    reset(){
        this.comboText.node.active = false;
        this.currentCombo = 0;
        this.currentScore = 0;
        this.scoreText.string = "0";
        this.updateText();
    }

    // update (dt) {}
}
