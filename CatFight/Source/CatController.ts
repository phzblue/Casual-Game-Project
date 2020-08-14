import GameController from "./GameController";
import Util from "./Util";
import SpawnController from "./SpawnController";

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
export default class CatController extends cc.Component {

    @property({
        type: cc.AudioClip
    })
    moveSFX: cc.AudioClip = null;

    @property(cc.SpriteFrame)
    defaultCatImage: cc.SpriteFrame = null;

    @property(cc.Node)
    star: cc.Node = null;

    maxColNum: number = 0;

    currentSpot: number = 0;
    isMoving: boolean = false;
    spawnController: SpawnController = null;

    onLoad() {
        this.spawnController = GameController.instance.spawnController.getComponent(SpawnController);
        this.maxColNum = this.spawnController.spawnPoint.length;
        this.currentSpot = 2;
    }

    start() {
        this.node.setPosition(this.spawnController.spawnPoint[this.currentSpot].position.x, this.node.position.y);
        //this.node.setPosition(this.spawnController.spawnPoint[this.currentSpot].position.x, this.node.position.y);
    }

    checkCatMovement(num) {
        if (!this.isMoving) {
            if (num < 0) {
                if (this.currentSpot > 0) {
                    this.currentSpot--;

                    this.checkNextPosIsLeg(-1);
                    this.node.children[0].scaleX = -1;
                    this.moveCat();
                    return true;
                }
            } if (num > 0) {
                if (this.currentSpot < this.maxColNum - 1) {
                    this.currentSpot++;

                    this.checkNextPosIsLeg(1);
                    this.node.children[0].scaleX = 1;
                    this.moveCat();
                    return true;
                }
            }
        }
        return false;
    }

    checkNextPosIsLeg(num) {
        if (this.currentSpot == GameController.instance.leftLegPos ||
            this.currentSpot == GameController.instance.rightLegPos) {

            if (num > 0) {
                if (this.currentSpot + 1 >= this.maxColNum) {
                    this.currentSpot--;
                    return;
                } else {
                    this.currentSpot++;
                    this.checkNextPosIsLeg(num);
                }
            } else {
                if (this.currentSpot - 1 < 0) {
                    this.currentSpot++;
                    return;
                } else {
                    this.currentSpot--;
                    this.checkNextPosIsLeg(num);
                }
            }
        }
    }

    moveCat() {
        if (!this.isMoving) {
            this.node.getComponentInChildren(cc.Animation).stop();
            this.node.children[0].getComponent(cc.Sprite).spriteFrame = this.defaultCatImage;
            this.node.children[1].getComponent(cc.Animation).stop();
            this.node.children[1].active = false;

            let moveAmount = this.spawnController.spawnPoint[this.currentSpot].position.x - this.node.position.x;
            let move = cc.moveBy(.2, moveAmount, 0);
            let stopAnim = cc.callFunc(() => {
                this.isMoving = false;
            });
            this.node.runAction(cc.sequence(move, stopAnim));
            this.isMoving = true;
            cc.audioEngine.playEffect(this.moveSFX, false);

            if (GameController.instance.isPowerActivated == true) {
                this.node.children[0].getComponent(cc.Animation).play("SuperIdleAnim");
            }

        }
    }

    resetCat() {

        this.node.children[0].getComponent(cc.Animation).stop();
        this.node.children[0].getComponent(cc.Sprite).spriteFrame = this.defaultCatImage;

        this.star.getComponent(cc.Animation).stop();
        this.star.active = false;

        this.node.setPosition(this.spawnController.spawnPoint[this.currentSpot].position.x, this.node.position.y);
    }

    playAnim() {
        this.node.children[0].getComponent(cc.Animation).play("scratchAnim");
        this.star.getComponent(cc.Animation).play();
        this.star.active = true;
    }

    playSuperAnim() {
        this.node.children[0].getComponent(cc.Animation).play("SuperScratchAnim");
        this.star.getComponent(cc.Animation).play();
        this.star.active = true;
    }

    stopSuperMode() {
        console.log("Stop");
        this.node.children[0].getComponent(cc.Animation).stop("SuperIdleAnim");
        this.node.children[0].getComponent(cc.Sprite).spriteFrame = this.defaultCatImage;
    }

    isCatNextToLeg() {

        let leftLegDiff = this.currentSpot - GameController.instance.leftLegPos;
        let rightLegDiff = this.currentSpot - GameController.instance.rightLegPos;

        if (GameController.instance.leftLegPos == -1) {
            leftLegDiff = -111;
        }

        if (GameController.instance.rightLegPos == -1) {
            rightLegDiff = -111;
        }

        if (Math.abs(leftLegDiff) == 1) {
            this.node.children[0].scaleX = -leftLegDiff;
            return true;

        } else if (Math.abs(rightLegDiff) == 1) {
            this.node.children[0].scaleX = -rightLegDiff;
            return true;
        }

        return false;
    }

    // update (dt) {}
}
