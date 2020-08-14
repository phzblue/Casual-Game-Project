// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import Util from "./Util";

const {ccclass, property} = cc._decorator;

@ccclass
export default class ScoreController extends cc.Component {

    combo:number = 0;
    score:number = 0;

    @property(cc.Prefab)
    scorePrefab:cc.Prefab = null;

    @property(cc.Label)
    scoreLabel:cc.Label = null;
    @property(cc.Label)
    comboLabel:cc.Label = null;

    @property(cc.Node)
    scoreContainer:cc.Node[] = [];

    tempNum:number = 0;

    addScore(){
        this.combo++;
        this.score += this.combo;

        this.scoreLabel.string = this.score.toString();
        this.comboLabel.string = this.combo.toString();

        this.comboLabel.node.parent.active = this.combo >=2;

        this.showScoreDisplay(this.combo);
    }

    deductScore(){
        this.combo = 0;
        this.score--;

        if(this.score < 0){
            this.score = 0;
        }

        this.comboLabel.node.parent.active = false;
        this.scoreLabel.string = this.score.toString();

        this.showScoreDisplay(-1);
    }

    showScoreDisplay(number:number){
        let scoreObj = cc.instantiate(this.scorePrefab);

        if(number < 0){
            scoreObj.color = cc.Color.RED;
            scoreObj.getComponent(cc.Label).string = number.toString();
        }else{
            scoreObj.getComponent(cc.Label).string = "+"+number.toString();
        }

        scoreObj.setParent(this.scoreContainer[this.tempNum % 2]);
        this.tempNum++;

        cc.tween(scoreObj)
        .by(1,{position: cc.v2(0,100)})
        .then( cc.fadeOut(0.5))
        .removeSelf()
        .start();
    }


}
