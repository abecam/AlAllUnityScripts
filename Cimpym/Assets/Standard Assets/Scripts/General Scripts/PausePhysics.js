#pragma strict

public var otherLock : Transform;
var clicked = false;
 
function OnMouseDown () {
 clicked = true;
}
 
function Start () {

}

function Update () {
   if (clicked)
	 {
       Time.timeScale = 0;  
       this.transform.position.z = -10;
       otherLock.position.z = 0;
   } 
}