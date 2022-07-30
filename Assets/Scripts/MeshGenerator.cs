using System.Collections.Generic;
using UnityEngine;

public enum Direction {
    Up,
    Down,
    North,
    East,
    South,
    West
}

public static class MeshGenerator {

    public static List<Vector3> MakeCubeFace(Vector3 pos, Direction dir) {
        if (dir == Direction.Up) return MakeCubeUpFace(pos);
        if (dir == Direction.Down) return MakeCubeDownFace(pos);
        if (dir == Direction.North) return MakeCubeNorthFace(pos);
        if (dir == Direction.East) return MakeCubeEastFace(pos);
        if (dir == Direction.South) return MakeCubeSouthFace(pos);
        if (dir == Direction.West) return MakeCubeWestFace(pos);

        return null;
    }

    public static List<Vector3> MakeCubeUpFace(Vector3 pos) {
        List<Vector3> face = new List<Vector3>();

        face.Add(pos + new Vector3(0, 1, 0));
        face.Add(pos + new Vector3(0, 1, 1));
        face.Add(pos + new Vector3(1, 1, 1));
        face.Add(pos + new Vector3(1, 1, 0));

        return face;
    }

    public static List<Vector3> MakeCubeDownFace(Vector3 pos) {
        List<Vector3> face = new List<Vector3>();

        face.Add(pos + new Vector3(0, 0, 0));
        face.Add(pos + new Vector3(1, 0, 0));
        face.Add(pos + new Vector3(1, 0, 1));
        face.Add(pos + new Vector3(0, 0, 1));

        return face;
    }

    public static List<Vector3> MakeCubeNorthFace(Vector3 pos) {
        List<Vector3> face = new List<Vector3>();

        face.Add(pos + new Vector3(0, 0, 1));
        face.Add(pos + new Vector3(1, 0, 1));
        face.Add(pos + new Vector3(1, 1, 1));
        face.Add(pos + new Vector3(0, 1, 1));

        return face;
    }

    public static List<Vector3> MakeCubeEastFace(Vector3 pos) {
        List<Vector3> face = new List<Vector3>();

        face.Add(pos + new Vector3(1, 0, 0));
        face.Add(pos + new Vector3(1, 1, 0));
        face.Add(pos + new Vector3(1, 1, 1));
        face.Add(pos + new Vector3(1, 0, 1));

        return face;
    }

    public static List<Vector3> MakeCubeSouthFace(Vector3 pos) {
        List<Vector3> face = new List<Vector3>();

        face.Add(pos + new Vector3(0, 0, 0));
        face.Add(pos + new Vector3(0, 1, 0));
        face.Add(pos + new Vector3(1, 1, 0));
        face.Add(pos + new Vector3(1, 0, 0));

        return face;
    }

    public static List<Vector3> MakeCubeWestFace(Vector3 pos) {
        List<Vector3> face = new List<Vector3>();

        face.Add(pos + new Vector3(0, 0, 0));
        face.Add(pos + new Vector3(0, 0, 1));
        face.Add(pos + new Vector3(0, 1, 1));
        face.Add(pos + new Vector3(0, 1, 0));

        return face;
    }

    public static List<int> AddCubeFaceTriangles(int currentVerticesNumber, int numberOfFaceAdded) {
        int numberOfVerticesPerFace = 4;
        int previousVerticesNumber = currentVerticesNumber - (numberOfFaceAdded * numberOfVerticesPerFace);
        List<int> triangles = new List<int>();

        for (int i = 0; i < numberOfFaceAdded; i++) {
            int triangleStartVertice = previousVerticesNumber + numberOfVerticesPerFace * i;
            triangles.AddRange(new int[] {
                triangleStartVertice, triangleStartVertice + 1, triangleStartVertice + 2,
                triangleStartVertice, triangleStartVertice + 2, triangleStartVertice + 3,
            });
        }
        return triangles;
    }

}


