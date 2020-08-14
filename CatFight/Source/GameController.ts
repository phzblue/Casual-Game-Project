import SpawnController from "./SpawnController";
import BoostController from "./BoostController";
import CatController from "./CatController";
import Util from "./Util";
import AnimationController from "./AnimationController";
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

const { ccclass, property } = cc._decorator;

@ccclass
export default class GameController extends cc.Component {

    static instance: GameController = null;

    @property({
        type: cc.AudioClip
    })
    normalAtk: cc.AudioClip = null;
    @property({
        type: cc.AudioClip
    })
    doubleAtk: cc.AudioClip = null;
    @property({
        type: cc.AudioClip
    })
    stunSFX: cc.AudioClip = null;


    @property(cc.Node)
    spawnController: cc.Node = null;
    @property(cc.Node)
    boostController: cc.Node = null;
    @property(cc.Node)
    cat: cc.Node = null;

    @property(cc.Prefab)
    dmgPrefab: cc.Prefab = null;
    @property(cc.Node)
    dmgContainer: cc.Node = null;

    @property(cc.Node)
    gameoverUI: cc.Node = null;
    @property(cc.Node)
    goScoreText: cc.Node = null;
    @property(cc.Node)
    goHiScoreText: cc.Node = null;
    @property(cc.Node)
    startUI: cc.Node = null;

    @property(cc.Node)
    boosterBar: cc.Node = null;

    @property(cc.Node)
    scoreText: cc.Node = null;
    @property(cc.Node)
    timerText: cc.Node = null;

    isGameOver: boolean = false;
    isPowerActivated: boolean = false;
    isStunned: boolean = false;

    currentScore: number = 0;
    timer: number = 60;
    hiScore: number = 0;

    spawnID: number = 0;
    legID: number = 0;
    timerID: number = 0;

    @property(cc.Node)
    leftLeg: cc.Node = null;
    @property(cc.Node)
    rightLeg: cc.Node = null;

    leftLegPos: number = 1;
    rightLegPos: number = 3;

    GOScoreText: string = "";
    GOHiScoreText: string = "";
    scoreIndex: number = 0;
    hiScoreIndex: number = 0;
    @property(cc.Button) button_Restart: cc.Button = null;
    @property(BoostController) boost: BoostController = null;

    sdkApp:SDKFunctions = null;

    onLoad() {
        cc.director.getCollisionManager().enabled = true;
        cc.director.getPhysicsManager().enabled = true;

        GameController.instance = this;
        this.sdkApp = this.getComponent(SDKFunctions);
        this.hiScore = cc.sys.localStorage.getItem("cat_hiscore") != null ? cc.sys.localStorage.getItem("cat_hiscore") : 0;
    }

    start() {
        this.resetGame();
        this.startGame();
    }

    startGame() {
        this.startUI.active = false;
        this.timerID = setInterval(() => {
            this.timer--;
            this.timerText.getComponent(cc.Label).string = this.secondToMinute(this.timer);
            if (this.timer <= 0) {
                this.displayGameOver();
            }
        }, 1000);

        this.spawnObject();

        this.startLegAction();
    }

    secondToMinute(number: number) {
        let min, sec;

        min = Math.floor(number / 60);
        sec = ("0" + Math.floor(number >= 60 ? number - 60 : number)).slice(-2)

        return min + ":" + sec;
    }

    resetGame() {
        this.button_Restart.interactable = false;
        this.gameoverUI.position = cc.v2(0, 1300);

        this.cat.getComponent(CatController).stopSuperMode();
        this.boost.superMode.stop();
        this.boost.superMode.node.active = false;

        this.gameoverUI.active = false;
        this.startUI.active = true;
        // AnimationController.instance.menuScreenAnim();
        this.currentScore = 0;
        this.timer = 60;

        this.boostController.getComponent(BoostController).reset();
        this.cat.getComponent(CatController).resetCat();

        this.scoreText.getComponent(cc.Label).string = "0";
        this.timerText.getComponent(cc.Label).string = this.secondToMinute(this.timer);

        this.isGameOver = false;
        this.isStunned = false;
    }

    startLegAction() {
        let randInterval = 0;
        this.legID = setInterval(() => {
            this.moveHumanLeg();
            randInterval = Util.getRandom(5000);
        }, 5000 + randInterval);
    }

    spawnObject() {
        let interval = 0;
        this.spawnID = setInterval(() => {
            this.spawnController.getComponent(SpawnController).spawnObject();
            interval = Util.getRandom(3000);
        }, 1500 + interval)
    }

    moveHumanLeg() {
        let currentCatSpot = this.cat.getComponent(CatController).currentSpot;
        let currentCatPos = this.spawnController.getComponent(SpawnController).spawnPoint[currentCatSpot];

        let leftLegDiff = Math.abs(currentCatSpot - this.leftLegPos);
        let rightLegDiff = Math.abs(currentCatSpot - this.rightLegPos);

        if (leftLegDiff == rightLegDiff || leftLegDiff < rightLegDiff) {
            var seq = cc.sequence(
                cc.moveBy(.5, 0, 500),
                cc.callFunc(() => {
                    this.leftLegPos = -1;
                    this.leftLeg.children[0].active = true;
                }),
                cc.delayTime(1),
                cc.moveBy(0.2, currentCatPos.x - this.leftLeg.position.x, 0),
                cc.moveBy(.5, 0, -500)
            );

            this.leftLeg.runAction(cc.sequence(seq, cc.callFunc(() => {
                this.leftLegPos = currentCatSpot;
                this.leftLeg.children[0].active = false;
            })));
        } else {
            var seq = cc.sequence(
                cc.moveBy(0.5, 0, 500),
                cc.callFunc(() => {
                    this.rightLegPos = -1;
                    this.rightLeg.children[0].active = true;
                }),
                cc.delayTime(0.5),
                cc.moveBy(0.2, currentCatPos.x - this.rightLeg.position.x, 0),
                cc.moveBy(.5, 0, -500)
            );
            this.rightLeg.runAction(cc.sequence(seq, cc.callFunc(() => {
                this.rightLegPos = currentCatSpot;
                this.rightLeg.children[0].active = false;

            })));
        }
    }

    tapHuman() {
        if (!this.isGameOver &&
            !this.cat.getComponent(CatController).isMoving
            && this.cat.getComponent(CatController).isCatNextToLeg()) {

            if (this.isPowerActivated == true) {
                this.cat.getComponent(CatController).playSuperAnim();
            }
            else {
                this.cat.getComponent(CatController).playAnim();
            }

            cc.audioEngine.playEffect(this.isPowerActivated ? this.doubleAtk : this.normalAtk, false);

            this.increaseScore();
            this.boostController.getComponent(BoostController).manipulateBoost(25);
        }
    }

    increaseScore() {
        let dmgIndicator = cc.instantiate(this.dmgPrefab);
        dmgIndicator.getComponent(cc.Label).string = (this.isPowerActivated ? 1 * 2 : 1).toString();
        dmgIndicator.setParent(this.dmgContainer);
        dmgIndicator.runAction(cc.sequence(cc.spawn(cc.moveBy(.5, 0, 80), cc.fadeOut(.5)), cc.callFunc(() => {
            dmgIndicator.destroy();
        })));

        this.currentScore += this.isPowerActivated ? 1 * 2 : 1;
        this.sdkApp.sendCurrentScore(this.currentScore);
        this.scoreText.getComponent(cc.Label).string = this.currentScore.toString();
    }

    stunCat() {
        if (this.isStunned != true) {
            cc.audioEngine.playEffect(this.stunSFX, false);

            this.cat.children[1].active = true;
            this.cat.children[1].getComponent(cc.Animation).play();
            this.isStunned = true;

            this.node.runAction(cc.sequence(cc.delayTime(1.5), cc.callFunc(() => {
                this.cat.children[1].active = false;
                this.isStunned = false;
            }, this)))
        }
    }

    displayAbortGame(){
        this.gameoverUI.getComponent(cc.Button).enabled = true;
        this.gameoverUI.getComponentInChildren(cc.Label).string = "Tap To Show Result Now!";
    }

    displayGameOver() {
        clearInterval(this.spawnID);
        clearInterval(this.legID);
        clearInterval(this.timerID);

        this.isGameOver = true;
        this.boostController.getComponent(BoostController).deplete = false;

        this.sdkApp.triggerGameOver();
        this.gameoverUI.active = true;

        cc.tween(this.gameoverUI)
            .call(() => {
                this.gameoverUI.active = true;

                if (this.currentScore > this.hiScore) {
                    cc.sys.localStorage.setItem("cat_hiscore", this.currentScore);
                    this.hiScore = this.currentScore;
                }

                this.GOScoreText = "SCORE: " + this.currentScore;
                this.GOHiScoreText = "HIGHSCORE: " + this.hiScore;

                this.goScoreText.getComponent(cc.Label).string = "";
                this.goHiScoreText.getComponent(cc.Label).string = "";

                this.scoreIndex = 0;
                this.hiScoreIndex = 0;
            })
            .to(1, { position: cc.v2(0, 0) }, { easing: 'cubicIn' })
            .to(0.5, { position: cc.v2(0, 100) }, { easing: 'cubicOut' })
            .to(0.5, { position: cc.v2(0.0) }, { easing: 'cubicIn' })
            .delay(0.2)
            .call(() => {
                //this.displayScores();
            })
            .start();
    }

    displayScores() {
        cc.tween(this.node)
            .delay(0.02)
            .call(() => {
                if (this.scoreIndex < this.GOScoreText.length) {
                    this.goScoreText.getComponent(cc.Label).string += this.GOScoreText.charAt(this.scoreIndex);
                    this.scoreIndex++;
                }

                if (this.hiScoreIndex < this.GOHiScoreText.length) {
                    this.goHiScoreText.getComponent(cc.Label).string += this.GOHiScoreText.charAt(this.hiScoreIndex);
                    this.hiScoreIndex++;
                }
            })
            .call(() => {
                if (this.scoreIndex < this.GOScoreText.length || this.hiScoreIndex < this.GOHiScoreText.length) {
                    this.displayScores();
                }

                else {
                    cc.tween(this.node)
                        .delay(0.5)
                        .call(() => {
                            this.button_Restart.interactable = true;
                        })
                        .start();
                }
            }).start();
    }

    // update (dt) {}
}
