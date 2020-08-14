import GameController from "./GameController";
import CatController from "./CatController";
import Animations from "./Animations";

// Learn TypeScript:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/typescript.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/reference/attributes.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/life-cycle-callbacks.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/life-cycle-callbacks.html

const { ccclass, property } = cc._decorator;

@ccclass
export default class BoostController extends cc.Component {

    @property({
        type: cc.AudioClip
    })
    boostSFX: cc.AudioClip = null;

    @property(cc.Node)
    boostUI: cc.Node = null;

    boosterBar: cc.ProgressBar = null;
    maxNum: number = 1000;
    currentNum: number = 0;

    fullHit: number = 0;
    deplete: boolean = false;

    @property(cc.Animation) superMode: cc.Animation = null;
    @property(CatController) cat: CatController = null;
    @property(Animations) animations: Animations = null;

    onLoad() {
        this.boosterBar = this.node.getComponent(cc.ProgressBar);
    }

    start() {

        setInterval(() => {
            if (!GameController.instance.isGameOver) {
                this.manipulateBoost(-3);
            }
        }, 1000);
    }

    reset() {
        this.boosterBar.progress = 0;
        this.deplete = false;
        this.fullHit = 0;
        this.currentNum = 0;
        GameController.instance.isPowerActivated = false;
    }

    manipulateBoost(num) {

        if (GameController.instance.isPowerActivated) {
            if (this.currentNum < 0) {
                this.boostUI.active = false;
                this.deplete = false;
                this.currentNum = 0;
                GameController.instance.isPowerActivated = false;

                this.cat.stopSuperMode();
                this.superMode.stop();
                this.superMode.node.active = false;
            }
        } else {
            if (this.currentNum >= this.maxNum) {
                this.fullHit++;

                if (this.fullHit > 5) {
                    cc.audioEngine.playEffect(this.boostSFX, false);
                    this.boostUI.active = true;
                    this.fullHit = 0;
                    this.deplete = true;
                    GameController.instance.isPowerActivated = true;

                    this.animations.onAnimCompleted();
                    this.superMode.node.active = true;
                    this.superMode.play();
                }
            } else {
                this.fullHit = 0;

                this.currentNum += num;
                this.boosterBar.progress = this.currentNum / this.maxNum;
            }
        }
    }

    update(dt) {
        if (this.deplete) {
            this.currentNum -= 1;
            this.boosterBar.progress = this.currentNum / this.maxNum;
        }
    }
}
