using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour
{
    private bool[,] isMade;
    private State[,] horEdges;
    private State[,] verEdges;
    private enum State{ closed=0, unset=1, open=2};
    public readonly int roomSize = 12;
    private List<GameObject> furniture;
    private int numEnemies;
    private int width;
    private int height;
    private Player p;
    private int numNPCs;
    private int numNPCsPlaced;
    private GameObject dialogueManager;
    private List<GameObject> objects;
    private List<Interact> NPCs;
    private Room[,] rooms;

    void Awake(){
        objects = new List<GameObject>();
        NPCs = new List<Interact>();
        dialogueManager = GameObject.Find("DialogueManager");
        furniture = new List<GameObject>();
        furniture.Add(Furniture.Get.Cabinet);
        furniture.Add(Furniture.Get.Chair);
        furniture.Add(Furniture.Get.Couch);
        furniture.Add(Furniture.Get.Crate);
        furniture.Add(Furniture.Get.DinnerTable);
        furniture.Add(Furniture.Get.DogBed);
        furniture.Add(Furniture.Get.Oven);
        furniture.Add(Furniture.Get.Plant);
        furniture.Add(Furniture.Get.Rug);
        furniture.Add(Furniture.Get.Table);
        furniture.Add(Furniture.Get.Wardrobe);
    }

    void Update(){
        if(NPCs.Count >= 3){
        for(int i = 0; i < NPCs.Count; i++){
            if((NPCs[i].interactable && NPCs[i].path != 3) || !NPCs[i].dialogueManager.GetComponent<dialogue>().notTalking()){ return; }
        }
        destroyAll();
        p.nextLevel();
        Destroy(this);
        }
    }
    private void destroyAll(){
        for(int i = 0; i < objects.Count; i++){
            Destroy(objects[i]);
        }
    }

    public void makeMap(int width, int height, int numMonsters, int numNPCs, Player p){
        this.rooms = new Room[width,height];
        this.numNPCs = numNPCs;
        this.p = p;
        this.width = width;
        this.height = height;
        numEnemies = numMonsters;
        numNPCsPlaced = 0;
        isMade = new bool[width,height];
        horEdges = new State[width,height+1];
        verEdges = new State[width+1,height];
        for(int i = 0; i < width; i++) for(int j = 0; j < height+1; j++){
            if(j==0 || j==height){
                horEdges[i,j] = State.closed;
            }else{
                horEdges[i,j] = State.unset;
            }
        }
        for(int i = 0; i < width+1; i++) for(int j = 0; j < height; j++){
            if(i==0 || i==width){
                verEdges[i,j] = State.closed;
            }else{
                verEdges[i,j] = State.unset;
            }
        }
        horEdges[(width-1)/2, 0] = State.open;
        Vector3 botLeft = new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z) - new Vector3(((width-1)/2)*roomSize,-roomSize,0);
        GameObject g = Instantiate(Rooms.Instance.startEnd);
        g.transform.Rotate(new Vector3(0,0,180));
        g.transform.position = p.transform.position;
        objects.Add(g);
        List<int> openPathXs = new List<int>();
        List<int> openPathYs = new List<int>();
        openPathXs.Add((width-1)/2);
        openPathYs.Add(0);
        while(openPathXs.Count != 0){
            int rand = Random.Range(0, openPathXs.Count);
            int x = openPathXs[rand];
            int y = openPathYs[rand];
            openPathXs.RemoveAt(rand);
            openPathYs.RemoveAt(rand);
            if(!isMade[x,y]){
            getRandomRoom(x,y,botLeft);
            isMade[x,y] = true;
            if(x >= 1 && !isMade[x-1,y] && verEdges[x,y] != State.closed){ openPathXs.Add(x-1); openPathYs.Add(y);}
            if(x < width-1 && !isMade[x+1,y] && verEdges[x+1,y] != State.closed){ openPathXs.Add(x+1); openPathYs.Add(y);}
            if(y >= 1 && !isMade[x,y-1] && horEdges[x,y] != State.closed){ openPathXs.Add(x); openPathYs.Add(y-1);}
            if(y < height-1 && !isMade[x,y+1] && horEdges[x,y+1] != State.closed){ openPathXs.Add(x); openPathYs.Add(y+1);}
            }
        }
        //Check if maze is too small, make it again if it is
        int roomCount = 0;
        for(int i = 0; i < width; i++) for(int j = 0; j < height; j++){
            if(isMade[i,j]){ roomCount++;}
        }
        if(roomCount < 0.9*width*height){ retry(width, height, numMonsters, numNPCs, p); return; }

        //Make furniture, NPCs and enemies
        int NPCrand = Random.Range(0, width*height);
        int Enemyrand = Random.Range(0, width*height);
        for(int i = 0; i < width; i++) for(int j = 0; j < height; j++){
            if(rooms[i,j] != null){
                Room r = rooms[i,j];
                int numFurniture = Random.Range(0, 3);
                for(int n = 0; n < numFurniture; n++){
                    makeFurniture(i,j,r,botLeft, g);
                }
                if((i+j*width + Enemyrand)%((int)roomCount/numEnemies) == 0){
                    makeEnemy(i,j,r,botLeft,g);
                }
                if((i+j*width + NPCrand)%((int)roomCount/numNPCs) == 0){
                    numNPCsPlaced++; //this is important for the path making
                    makeNPC(i,j,r,botLeft,g); 
                }
            }
        }
    }

    private void retry(int width, int height, int numMonsters, int numNPCs, Player p){
        destroyAll();
        objects = new List<GameObject>();
        NPCs = new List<Interact>();
        makeMap(width, height, numMonsters, numNPCs, p);
    }

//Returns a random room you can enter from below
    public GameObject getRandomRoom(int x, int y, Vector3 pos){ //x and y go from 0 to width-1 or height-1
        List<Room> roomTypes = new List<Room>();
        roomTypes.Add(new corridorRoom(this));
        roomTypes.Add(new rightTurnRoom(this));
        roomTypes.Add(new TRoom(this));
        roomTypes.Add(new crossRoom(this));
        int loop = 0;
        GameObject g;
        while(true){
            loop++;
            if(loop > 100){ break; }
            int rand = Random.Range(0, roomTypes.Count);
            g = makeRoom(x,y,roomTypes[rand],pos);
            if(g != null){
            return g;
            }
        }
        //If it can only be a dead end
        Room r = new deadEndRoom(this);
        g = makeRoom(x,y,r,pos);
        if(g == null){ Debug.Log("Mapmaking failed");}
        return g;
    }

    private GameObject makeRoom(int x, int y, Room r, Vector3 pos){
            List<int> rotations = new List<int>();
            for(int i = 0; i < 360; i+=90){
                r.rotate(i);
                if((verEdges[x,y] == State.unset || r.leftOpen() == (verEdges[x,y] == State.open))
                && (verEdges[x+1,y] == State.unset || r.rightOpen() == (verEdges[x+1,y] == State.open))
                 && (horEdges[x,y] == State.unset || r.downOpen() == (horEdges[x,y] == State.open))
                 && (horEdges[x,y+1] == State.unset || r.upOpen() == (horEdges[x,y+1] == State.open))){
                    rotations.Add(i);
                }
            }
            if(rotations.Count != 0){
                int rand = Random.Range(0,rotations.Count);
                GameObject g = Instantiate(r.prefab);
                g.transform.Rotate(new Vector3(0,0,-rotations[rand]));
                r.rotate(rotations[rand]);
                verEdges[x,y] = r.leftOpen()?State.open:State.closed;
                verEdges[x+1,y] = r.rightOpen()?State.open:State.closed;
                horEdges[x,y] = r.downOpen()?State.open:State.closed;
                horEdges[x,y+1] = r.upOpen()?State.open:State.closed;
                g.transform.position = pos + new Vector3(roomSize*x, roomSize*y,0);
                objects.Add(g);
                rooms[x,y] = r;
                return g;
            }else{
                return null;
            }
    }

    private void makeFurniture(int x, int y, Room r, Vector3 pos, GameObject roomObj){
        GameObject item = Instantiate(furniture[Random.Range(0, furniture.Count)]);
        objects.Add(item);
        while(true){
             item.transform.position = pos + new Vector3(roomSize*x, roomSize*y,0) + new Vector3(Random.Range(r.minX(), r.maxX()), Random.Range(r.minY(), r.maxY()));
            item.transform.Rotate(new Vector3(0,0,Random.Range(0,360)));
            Physics.SyncTransforms();
            for(int i = 0; i < objects.Count; i++){
                Collider2D[] col = objects[i].GetComponentsInChildren<Collider2D>();
                for(int j = 0; j < col.Length; j++){
                    if(col[j] != item.GetComponent<Collider2D>()){
                        if(col[j].bounds.Intersects(item.GetComponent<Collider2D>().bounds)){
                           continue;
                      }
                    }
                }
            }
            break;
        }
    }

    private void makeEnemy(int x, int y, Room r, Vector3 pos, GameObject roomObj){
        GameObject item = Instantiate(Furniture.Get.Enemy);
        item.GetComponent<Illness>().player = p.transform;
        objects.Add(item);
        while(true){
             item.transform.position = pos + new Vector3(roomSize*x, roomSize*y,0) + new Vector3(Random.Range(r.minX(), r.maxX()), Random.Range(r.minY(), r.maxY()));
            item.transform.Rotate(new Vector3(0,0,Random.Range(0,360)));
            Physics.SyncTransforms();
            for(int i = 0; i < objects.Count; i++){
                Collider2D[] col = objects[i].GetComponentsInChildren<Collider2D>();
                for(int j = 0; j < col.Length; j++){
                    if(col[j] != item.GetComponent<Collider2D>()){
                        if(col[j].bounds.Intersects(item.GetComponent<Collider2D>().bounds)){
                           continue;
                      }
                    }
                }
            }
            break;
        }
    }

    private void makeNPC(int x, int y, Room r, Vector3 pos, GameObject roomObj){
        GameObject item = Instantiate(Furniture.Get.NPC);
        item.GetComponent<Interact>().dialogueManager = this.dialogueManager;
        item.GetComponent<Interact>().path = (this.numNPCsPlaced-1);
                NPCs.Add(item.GetComponent<Interact>());
        objects.Add(item);
        while(true){
             item.transform.position = pos + new Vector3(roomSize*x, roomSize*y,0) + new Vector3(Random.Range(r.minX(), r.maxX()), Random.Range(r.minY(), r.maxY()));
            item.transform.Rotate(new Vector3(0,0,Random.Range(0,360)));
            Physics.SyncTransforms();
            for(int i = 0; i < objects.Count; i++){
                Collider2D[] col = objects[i].GetComponentsInChildren<Collider2D>();
                for(int j = 0; j < col.Length; j++){
                    if(col[j] != item.GetComponent<Collider2D>()){
                        if(col[j].bounds.Intersects(item.GetComponent<Collider2D>().bounds)){
                           continue;
                      }
                    }
                }
            }
            break;
        }
    }

    class Room{
        protected MapMaker m;
protected int[] open;
protected Vector2 botLeft;
protected Vector2 botRight;
protected Vector2 topLeft;
protected Vector2 topRight;
public GameObject prefab;
 public int rotation = 0;
 public void rotate(int degrees){
    for(int i = 0; i < rotation; i++){
        botLeft = new Vector2(botLeft.y, -botLeft.x);
        botRight = new Vector2(botRight.y, -botRight.x);
        topLeft = new Vector2(topLeft.y, -topLeft.x);
        topRight = new Vector2(topRight.y, -topRight.x);
    }
    degrees = (degrees + 360)%360;
    degrees = degrees - 360;
    degrees /= 90;
    rotation = -degrees;
    for(int i = 0; i < rotation; i++){
        botLeft = new Vector2(-botLeft.y, botLeft.x);
        botRight = new Vector2(-botRight.y, botRight.x);
        topLeft = new Vector2(-topLeft.y, topLeft.x);
        topRight = new Vector2(-topRight.y, topRight.x);
    }
 }
     public bool leftOpen(){
        return open[(0 + rotation)%4]==1;
    }
         public bool upOpen(){
        return open[(1 + rotation)%4]==1;
    }
         public bool rightOpen(){
        return open[(2 + rotation)%4]==1;
    }
         public bool downOpen(){
        return open[(3 + rotation)%4]==1;
    }
    public float minX(){
        return Mathf.Min(botLeft.x, botRight.x, topLeft.x, topRight.x);
    }
    public float maxX(){
        return Mathf.Max(botLeft.x, botRight.x, topLeft.x, topRight.x);
    }
        public float minY(){
        return Mathf.Min(botLeft.y, botRight.y, topLeft.y, topRight.y);
    }
        public float maxY(){
        return Mathf.Max(botLeft.y, botRight.y, topLeft.y, topRight.y);
    }
}
class TRoom : Room{
    public TRoom(MapMaker m){ 
        this.open = new int[]{1,0,1,1};
        prefab = Rooms.Instance.Tee;
        botLeft = new Vector2(-2,-1);
        botRight = new Vector2(2,-1);
        topLeft = new Vector2(-2,1);
        topRight = new Vector2(2,1);
    }
}
class deadEndRoom : Room{
    public deadEndRoom(MapMaker m){ 
        this.open = new int[]{0,0,0,1};
        prefab = Rooms.Instance.deadEnd;
        botLeft = new Vector2(-1,-2);
        botRight = new Vector2(1,-2);
        topLeft = new Vector2(-1,0);
        topRight = new Vector2(1,0);
    }
}
class rightTurnRoom : Room{
    public rightTurnRoom(MapMaker m){ 
        this.open = new int[]{0,0,1,1};
        prefab = Rooms.Instance.rightTurn;
        botLeft = new Vector2(-1,-2);
        botRight = new Vector2(1,-2);
        topLeft = new Vector2(-1,1);
        topRight = new Vector2(1,1);
    }
}
class corridorRoom : Room{
    public corridorRoom(MapMaker m){ 
        this.open = new int[]{0,1,0,1};
        prefab = Rooms.Instance.corridor;
        botLeft = new Vector2(-1,-2);
        botRight = new Vector2(1,-2);
        topLeft = new Vector2(-1,2);
        topRight = new Vector2(1,2);
    }
}
class crossRoom : Room{
    public crossRoom(MapMaker m){ 
        this.open = new int[]{1,1,1,1};
        prefab = Rooms.Instance.cross;
        botLeft = new Vector2(-1,-2);
        botRight = new Vector2(1,-2);
        topLeft = new Vector2(-1,2);
        topRight = new Vector2(1,2);
    }
}
class startEndRoom : Room{
    public startEndRoom(MapMaker m){ 
        this.open = new int[]{0,0,0,1};
        prefab = Rooms.Instance.startEnd;
        botLeft = new Vector2(-2,-1);
        botRight = new Vector2(2,-1);
        topLeft = new Vector2(-2,2);
        topRight = new Vector2(2,2);
    }
}

}