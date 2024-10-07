using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wireframe : MonoBehaviour {

	LineRenderer[] lineRenderer;


	Vector3[] spreadPosition = new Vector3[25];
	// Use this for initialization
	void Start () {
		
		lineRenderer = this.GetComponentsInChildren<LineRenderer> ();

		for (int i = 0; i <6; i++)
		{
			lineRenderer [i].widthMultiplier = 0.01f;
			lineRenderer [i].positionCount = 10;
		}

	}



	public void AssignSpreadPos(int positionNumber, float x, float y)
	{
		spreadPosition [positionNumber] = new Vector3 (x, y, -0.5f);
	}

	// 4 = center point (original target pos)
	//
	//	0 --- 1 --- 2			Vert order in quad	
	//	|     |     |			
	//	3 --- 4 --- 5			3---2
	//	|     |     |			|	|
	//	6 --- 7 --- 8			0---1
	//
	//  EXTENDED
	//  
	//	9	10	11	12	13
	//	14	0	1	2	15
	//	16	3	4	5	17
	//	18	6	7	8	19
	//	20	21	22	23	24
	//

	//
	public	void ApplyPositionsToMeshes ()
	{	
		
		lineRenderer [0].SetPositions (new Vector3[10] {
			spreadPosition [9],
			spreadPosition [10],
			spreadPosition [11],
			spreadPosition [12],
			spreadPosition [13],
			spreadPosition [15],
			spreadPosition [2],
			spreadPosition [1],
			spreadPosition [0],
			spreadPosition [14]
		});

		lineRenderer [1].SetPositions (new Vector3[10] {
			spreadPosition [16],
			spreadPosition [3],
			spreadPosition [4],
			spreadPosition [5],
			spreadPosition [17],
			spreadPosition [19],
			spreadPosition [8],
			spreadPosition [7],
			spreadPosition [6],
			spreadPosition [18]
		});

		lineRenderer [2].SetPositions (new Vector3[10] {
			spreadPosition [18],
			spreadPosition [6],
			spreadPosition [7],
			spreadPosition [8],
			spreadPosition [19],
			spreadPosition [24],
			spreadPosition [23],
			spreadPosition [22],
			spreadPosition [21],
			spreadPosition [20]
		});

		lineRenderer [3].SetPositions (new Vector3[10] {
			spreadPosition [9],
			spreadPosition [14],
			spreadPosition [16],
			spreadPosition [18],
			spreadPosition [20],
			spreadPosition [21],
			spreadPosition [6],
			spreadPosition [3],
			spreadPosition [0],
			spreadPosition [10]
		}); 

		lineRenderer [4].SetPositions (new Vector3[10] {
			spreadPosition [11],
			spreadPosition [1],
			spreadPosition [4],
			spreadPosition [7],
			spreadPosition [22],
			spreadPosition [23],
			spreadPosition [8],
			spreadPosition [5],
			spreadPosition [2],
			spreadPosition [12]
		});

		lineRenderer [5].SetPositions (new Vector3[10] {
			spreadPosition [12],
			spreadPosition [2],
			spreadPosition [5],
			spreadPosition [8],
			spreadPosition [23],
			spreadPosition [24],
			spreadPosition [19],
			spreadPosition [17],
			spreadPosition [15],
			spreadPosition [13]
		});



	}
}























		/*
		topLeftMesh.Clear ();
		topLeftMesh.vertices = new Vector3[4]{
			spreadPosition[3], spreadPosition[4], spreadPosition[1], spreadPosition[0]};
		topLeftMesh = FixMesh (topLeftMesh);

		topRightMesh.Clear ();
		topRightMesh.vertices = new Vector3[4]{
			spreadPosition[4], spreadPosition[5], spreadPosition[2], spreadPosition[1]};
		topRightMesh = FixMesh (topRightMesh);

		botLeftMesh.Clear ();
		botLeftMesh.vertices = new Vector3[4]{
			spreadPosition[6], spreadPosition[7], spreadPosition[4], spreadPosition[3]};
		botLeftMesh = FixMesh (botLeftMesh);

		botRightMesh.Clear ();
		botRightMesh.vertices = new Vector3[4]{
			spreadPosition[7], spreadPosition[8], spreadPosition[5], spreadPosition[4]};
		botRightMesh = FixMesh (botRightMesh);
	
	}

	Mesh FixMesh(Mesh mesh)
	{
		int[] tri = new int[6] {0,2,1,0,3,2};

		mesh.triangles = tri;

		Vector2[] uv = new Vector2[4];

		uv [0] = new Vector2 (0,0);
		uv [1] = new Vector2 (1,0);
		uv [2] = new Vector2 (1,1);	
		uv [3] = new Vector2 (0,1);
		mesh.uv = uv;

		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();
		return mesh;
	}


	public void RotateWireframeAroundPoint(Vector3 point, float angle)
	{
		topLeft.transform.RotateAround (point, Vector3.forward, angle);
		topRight.transform.RotateAround (point, Vector3.forward, angle);
		botLeft.transform.RotateAround (point, Vector3.forward, angle);
		botRight.transform.RotateAround (point, Vector3.forward, angle);
	}
}




/*
	Vector3[] EditVertices( Vector3[] oldVerts, Vector3[] vertChange)
	{
		Vector3[] newVerts = new Vector3[] {
			new Vector3(oldVerts[0].x +vertChange[0].x, oldVerts[0].y+vertChange[0].y, oldVerts[0].z), 
			new Vector3(oldVerts[1].x +vertChange[1].x, oldVerts[1].y+vertChange[1].y, oldVerts[0].z), 
			new Vector3(oldVerts[2].x +vertChange[2].x, oldVerts[2].y+vertChange[2].y, oldVerts[0].z), 
			new Vector3(oldVerts[3].x +vertChange[3].x, oldVerts[3].y+vertChange[3].y, oldVerts[0].z), 
		};

		return newVerts;
	}

	Vector3[] EditVertices( Vector3[] oldVerts,  Vector3 vertChange0, Vector3 vertChange1, Vector3 vertChange2, Vector3 vertChange3)
	{
		Vector3[] newVerts = new Vector3[] {
			new Vector3(oldVerts[0].x +vertChange0.x, oldVerts[0].y+vertChange0.y, oldVerts[0].z), 
			new Vector3(oldVerts[1].x +vertChange1.x, oldVerts[1].y+vertChange1.y, oldVerts[0].z), 
			new Vector3(oldVerts[2].x +vertChange2.x, oldVerts[2].y+vertChange2.y, oldVerts[0].z), 
			new Vector3(oldVerts[3].x +vertChange3.x, oldVerts[3].y+vertChange3.y, oldVerts[0].z), 
		};

		return newVerts;
	}

	Vector3 EditVert(Vector3[] oldVerts, int vectorNumber, Vector3 vectorValue)
	{
		Vector3[] newVerts = oldVerts;

		newVerts [vectorNumber] = vectorValue;
			
		return newVerts;
	}

*/
