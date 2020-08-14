// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

const {ccclass, property} = cc._decorator;

@ccclass
export default class Ship extends cc.Component {

    @property(cc.Prefab)
    bulletPrefab:cc.Prefab = null;
    @property(cc.Node)
    bulletContainer:cc.Node = null;

    canShoot:boolean = false;

    shoot(){
        let bullet = cc.instantiate(this.bulletPrefab);
        bullet.setParent(this.bulletContainer);
        bullet.getComponent(cc.RigidBody).linearVelocity = cc.v2(0,2000);

        this.node.getChildByName("Firehit_texture").getComponent(cc.ParticleSystem).resetSystem();
    }

    // update (dt) {}
}
