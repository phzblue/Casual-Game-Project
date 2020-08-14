import Util from "./Util";
import GameController from "./GameController";

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
export default class ObjectGenerator extends cc.Component {

    @property(cc.Node)
    spawnNodes:cc.Node = null;

    @property(cc.Prefab)
    coinPrefab:cc.Prefab = null;

    @property(cc.Prefab)
    obstaclePrefab:cc.Prefab = null;

    maxSpeed:number = 300;
    minSpeed:number = 100;

    spawnTimer:any = 2;
    isSpawn:boolean = false;
    spawnInterval:any = 0;

    update(dt){

        if(!GameController.instance.isGameOver){
            this.spawnTimer -= dt;
            if(!GameController.instance.isGameOver && this.isSpawn && this.spawnTimer <= 0){
                this.spawnObjects();
    
                if(this.maxSpeed < 800){
                    this.minSpeed += 20;
                    this.maxSpeed += 20;
                }

                this.spawnTimer = 2 - (this.spawnInterval < 1.5 ? this.spawnInterval+=0.1 : this.spawnInterval);
            }
        }
    }

    startGenerating(){
        this.isSpawn = true;
    }

    reset(){
        this.isSpawn = false;
        this.spawnTimer = 2;
        this.spawnInterval = 0;

        this.unscheduleAllCallbacks();

        this.spawnNodes.children.forEach(element => {
            element.destroyAllChildren();            
        });

        this.maxSpeed = 300;
        this.minSpeed = 100;
    }

    spawnObjects(){
        let positions:any[] = this.spawnNodes.children.slice(0,this.spawnNodes.children.length);

        let obstacleNum = 4;
        let coinNum = 2;

        for(let i = 0; i < obstacleNum; i++){
            let posObj:cc.Node = this.pickPosition(positions);

            let obstacle = cc.instantiate(this.obstaclePrefab);
            let ran = Util.getRandomFloat(1,.5);
            obstacle.setScale(ran);

            obstacle.getComponent(cc.RigidBody).linearVelocity = cc.v2(0, Util.getRandom(this.minSpeed,this.maxSpeed));
            posObj.addChild(obstacle);
        }

        for(let i = 0; i < coinNum; i++){
            let posObj:cc.Node = this.pickPosition(positions);

            let coin = cc.instantiate(this.coinPrefab);
            coin.getComponent(cc.RigidBody).linearVelocity = cc.v2(0, Util.getRandom(this.minSpeed,this.maxSpeed));
            posObj.addChild(coin);
        }
    }

    pickPosition(array:any[]){
        Util.shuffleArray(array)
        return array.shift();
    }
}
