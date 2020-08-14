import Util from "./Util";

// Learn TypeScript:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/typescript.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/typescript.html
// Learn Attribute:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/reference/attributes.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/reference/attributes.html
// Learn life-cycle callbacks:
//  - [Chinese] https://docs.cocos.com/creator/manual/zh/scripting/life-cycle-callbacks.html
//  - [English] http://www.cocos2d-x.org/docs/creator/manual/en/scripting/life-cycle-callbacks.html

const {ccclass, property} = cc._decorator;

@ccclass
export default class SpawnController extends cc.Component {

    @property([cc.Prefab])
    objectPrefab:cc.Prefab[] = [];

    @property([cc.Node])
    spawnPoint:cc.Node[] = [];

    spawnObject(){
        let selectedSpot = Util.getRandom(this.spawnPoint.length-1);

        let newObj = cc.instantiate(this.objectPrefab[Util.getRandom(this.objectPrefab.length-1)])
        newObj.setParent(this.spawnPoint[selectedSpot]);
    }

    // update (dt) {}
}
