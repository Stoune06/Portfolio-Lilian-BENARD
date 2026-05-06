using System;
using System.Collections.Generic;
using UnityEngine;

public static class CubeManager
{
    private static List<Cube> _AllCubes = new List<Cube>();

    public static void RegisterCube(Cube pCube)
    {
        if (!_AllCubes.Contains(pCube))
        {
            _AllCubes.Add(pCube);
        }
    }

    public static void UnregisterCube(Cube pCube)
    {
        _AllCubes.Remove(pCube);
    }

    public static void ClearAllCubes()
    {
        for (int i = _AllCubes.Count - 1; i >= 0; i--)
        {
            if (_AllCubes[i] != null)
            {
                _AllCubes[i].DestroyCube();
            }
        }
        _AllCubes.Clear();
    }

    public static int GetCubeCount()
    {
        return _AllCubes.Count;
    }

    public static List<Cube> GetAllCubes()
    {
        return new List<Cube>(_AllCubes);
    }
}

