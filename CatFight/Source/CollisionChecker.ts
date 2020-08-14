import Util from "./Util";
import GameController from "./GameController";
import CatController from "./CatController";
import AnimationController from "./AnimationController";

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
export default class CollisionChecker extends cc.Component {

    onCollisionEnter(other: cc.Collider, self: cc.Collider) {
        if (self.name.toLowerCase().includes("cat")) {
            if (other.name.toLowerCase().includes("shoe")) {
                if (!GameController.instance.cat.getComponent(CatController).checkCatMovement(-1)) {
                    GameController.instance.cat.getComponent(CatController).checkCatMovement(1)
                }

                AnimationController.instance.shakeCameraAnim();
            }

            else {
                cc.tween(other.node)
                    .call(() => {
                        let random = Util.getRandom(2)

                        switch (random) {
                            case 0:
                                other.node.getComponent(cc.RigidBody).applyLinearImpulse(cc.v2(500, 500), cc.v2(0, 100), true);
                                break;
                            case 1:
                                other.node.getComponent(cc.RigidBody).applyLinearImpulse(cc.v2(-500, 500), cc.v2(0, 100), true);
                                break;
                        }
                    })
                    .parallel(
                        cc.tween().to(0.5, { angle: 720 }),
                        cc.tween().to(0.5, { opacity: 0 })
                    ).start();
            }
            GameController.instance.stunCat();
            AnimationController.instance.hitEffectAnim();

        } else if (self.name.toLowerCase().includes("despawn")) {
            other.node.destroy();
        }
    }
}
