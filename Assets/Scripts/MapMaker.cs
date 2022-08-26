using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour
{
    private bool[,] isMade;
    private State[,] horEdges;
    private State[,] verEdges;
    private enum State{ closed=0, unset=1, open=2};
    public int roomSize = 12;

    public void makeMap(int width, int height, int numMonsters, Player p){
        Room r = new Room();
        r.rotate(270);
        Debug.Log("Rotation = " + r.rotation);
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
            getRandomRoom(x,y,botLeft);
            isMade[x,y] = true;
            if(x >= 1 && !isMade[x-1,y] && verEdges[x,y] != State.closed){ openPathXs.Add(x-1); openPathYs.Add(y);}
            if(x < width-1 && !isMade[x+1,y] && verEdges[x+1,y] != State.closed){ openPathXs.Add(x+1); openPathYs.Add(y);}
            if(y >= 1 && !isMade[x,y-1] && horEdges[x,y] != State.closed){ openPathXs.Add(x); openPathYs.Add(y-1);}
            if(y < height-1 && !isMade[x,y+1] && horEdges[x,y+1] != State.closed){ openPathXs.Add(x); openPathYs.Add(y+1);}
        }
    }

//Returns a random room you can enter from below
    public GameObject getRandomRoom(int x, int y, Vector3 pos){ //x and y go from 0 to width-1 or height-1
        Room r;
        List<int> rotations;
        List<Room> roomTypes = new List<Room>();
        roomTypes.Add(new corridorRoom(this));
        roomTypes.Add(new rightTurnRoom(this));
        roomTypes.Add(new TRoom(this));
        roomTypes.Add(new crossRoom(this));
        int loop = 0;
        while(true){
            loop++;
            if(loop > 100){ break; }
            int rand = Random.Range(0, roomTypes.Count);
            r = roomTypes[rand];
            rotations = new List<int>();
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
                rand = Random.Range(0,rotations.Count);
                GameObject g = Instantiate(r.prefab);
                g.transform.Rotate(new Vector3(0,0,-rotations[rand]));
                r.rotate(rotations[rand]);
                verEdges[x,y] = r.leftOpen()?State.open:State.closed;
                verEdges[x+1,y] = r.rightOpen()?State.open:State.closed;
                horEdges[x,y] = r.downOpen()?State.open:State.closed;
                horEdges[x,y+1] = r.upOpen()?State.open:State.closed;
                g.transform.position = pos + new Vector3(roomSize*x, roomSize*y,0);
                return g;
            }
        }
        //If it can only be a dead end
        r = new deadEndRoom(this);
        rotations = new List<int>();
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
                return g;
            }
            return null;
    }

    class Room{
        protected MapMaker m;
protected int[] open;
public GameObject prefab;
 public int rotation = 0;
 public void rotate(int degrees){
    degrees = (degrees + 360)%360;
    degrees = degrees - 360;
    degrees /= 90;
    rotation = -degrees;
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
}
class TRoom : Room{
    public TRoom(MapMaker m){ 
        this.open = new int[]{1,0,1,1};
        prefab = Rooms.Instance.Tee;
    }
}
class deadEndRoom : Room{
    public deadEndRoom(MapMaker m){ 
        this.open = new int[]{0,0,0,1};
        prefab = Rooms.Instance.deadEnd;
    }
}
class rightTurnRoom : Room{
    public rightTurnRoom(MapMaker m){ 
        this.open = new int[]{0,0,1,1};
        prefab = Rooms.Instance.rightTurn;
    }
}
class corridorRoom : Room{
    public corridorRoom(MapMaker m){ 
        this.open = new int[]{0,1,0,1};
        prefab = Rooms.Instance.corridor;
    }
}
class crossRoom : Room{
    public crossRoom(MapMaker m){ 
        this.open = new int[]{1,1,1,1};
        prefab = Rooms.Instance.cross;
    }
}
class startEndRoom : Room{
    public startEndRoom(MapMaker m){ 
        this.open = new int[]{0,0,0,1};
        prefab = Rooms.Instance.startEnd;
    }
}

}