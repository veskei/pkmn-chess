using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Files
{
    None = 0,
    a = 1,
    b = 2,
    c = 3,
    d = 4,
    e = 5,
    f = 6,
    g = 7,
    h = 8
}


public class Square : MonoBehaviour
{
    public Piece current_piece;
    public int rank;
    public int file;
    public string sq_name;
}
