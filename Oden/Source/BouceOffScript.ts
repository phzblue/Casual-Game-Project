// Learn TypeScript:
//  - https://docs.cocos.com/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - https://docs.cocos.com/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - https://docs.cocos.com/creator/manual/en/scripting/life-cycle-callbacks.html

import Util from "./Util";

const {ccclass, property} = cc._decorator;

@ccclass
export default class BouceOffScript extends cc.Component {

    onCollisionEnter(other:cc.Collider, self:cc.Collider){
        if(other.node.name.toLowerCase().includes("oden") && other.node.parent.name.toLowerCase().includes("new")){
            let oden = other.node;
            if(oden.getComponent(cc.RigidBody) != null && oden.getComponent(cc.BoxCollider) != null){
                cc.tween(oden)
                .call(()=>{
                    oden.getComponent(cc.BoxCollider).enabled = false;
                })
                .then(cc.jumpBy(0.5,cc.v2(100*Util.getRandomArrayObject([1,-1]),0),150,1))
                .call(()=>{
                    oden.getComponent(cc.BoxCollider).enabled = true;
                })
                .start();
            }
        }
    }


}
