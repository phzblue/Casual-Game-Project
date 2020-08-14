// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import GameController from "./GameController";
import Util from "./Util";

const {ccclass, property} = cc._decorator;

@ccclass
export default class ScoreController extends cc.Component {

    @property(cc.Prefab)
    scoreDisplayPrefab:cc.Prefab = null;
    @property(cc.Label)
    scoreLabel:cc.Label = null;
    @property(cc.Label)
    comboLabel:cc.Label = null;

    combo:number = 0;
    currentScore:number = 0;

    addScore(num:number){
        let scoreDisplay = cc.instantiate(this.scoreDisplayPrefab);

        let scoreTotal = "0";

        if(num < 0){
            scoreDisplay.color = cc.Color.RED;
            this.combo = 0;

            if(this.currentScore > 0){
                this.currentScore--;
            }
            this.comboLabel.node.active = false;
            scoreTotal = "-1";
        }else{
            this.combo++;
            this.currentScore += (num*this.combo);

            this.comboLabel.string = "COMBO x"+this.combo;
            this.comboLabel.node.active = this.combo>=2
            scoreTotal = "+"+(num*this.combo);
        }

        scoreDisplay.getComponent(cc.Label).string = scoreTotal;

        this.scoreLabel.string = this.currentScore.toString();

        scoreDisplay.setParent(GameController.instance.slayer);
        scoreDisplay.setPosition(Util.getRandom(150,-150),0)

        cc.tween(scoreDisplay)
        .parallel(
            cc.moveBy(0.3,cc.v2(0,100)),
            cc.fadeIn(0.3)
        )
        .delay(.5)
        .then(cc.fadeOut(0.3))
        .removeSelf()
        .start();
    }

    // update (dt) {}
}