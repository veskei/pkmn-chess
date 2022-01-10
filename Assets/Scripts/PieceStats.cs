using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability
{
    public string ability_name;
    public int ability_max;
    public int ability_min;
    public bool defense = false;
    public bool weakened = false;
    public bool defensive_strike = false;
}
public class PieceUnit
{
    public string name;
    public int health;
    public Ability[] abilities = new Ability[4];
    public bool is_weakened = false;
    public bool is_defensive = false;
    public bool is_def_strike = false;
}
public class PieceStats : MonoBehaviour
{
    public PieceUnit[] piece_stats = new PieceUnit[7];

    // Start is called before the first frame update
    void Start()
    {
        // Pawn Data
        {
            piece_stats[1] = new PieceUnit();
            piece_stats[1].name = "Pawn";
            piece_stats[1].health = 15;

            piece_stats[1].abilities[0] = new Ability();
            piece_stats[1].abilities[0].ability_name = "Strike";
            piece_stats[1].abilities[0].ability_max = 10;
            piece_stats[1].abilities[0].ability_min = 7;

            piece_stats[1].abilities[1] = new Ability();
            piece_stats[1].abilities[1].ability_name = "Block";
            piece_stats[1].abilities[1].ability_max = 8;
            piece_stats[1].abilities[1].ability_min = 5;
            piece_stats[1].abilities[1].defense = true;
        }

        // Rook Data
        {
            piece_stats[2] = new PieceUnit();
            piece_stats[2].name = "Rook";
            piece_stats[2].health = 30;

            piece_stats[2].abilities[0] = new Ability();
            piece_stats[2].abilities[0].ability_name = "Strike";
            piece_stats[2].abilities[0].ability_max = 15;
            piece_stats[2].abilities[0].ability_min = 12;

            piece_stats[2].abilities[1] = new Ability();
            piece_stats[2].abilities[1].ability_name = "Block";
            piece_stats[2].abilities[1].ability_max = 12;
            piece_stats[2].abilities[1].ability_min = 8;
            piece_stats[2].abilities[1].defense = true;

            piece_stats[2].abilities[2] = new Ability();
            piece_stats[2].abilities[2].ability_name = "Bash";
            piece_stats[2].abilities[2].ability_max = 25;
            piece_stats[2].abilities[2].ability_min = 18;
            piece_stats[2].abilities[2].weakened = true;
        }

        // Knight Data
        {
            piece_stats[3] = new PieceUnit();
            piece_stats[3].name = "Knight";
            piece_stats[3].health = 22;

            piece_stats[3].abilities[0] = new Ability();
            piece_stats[3].abilities[0].ability_name = "Strike";
            piece_stats[3].abilities[0].ability_max = 15;
            piece_stats[3].abilities[0].ability_min = 12;

            piece_stats[3].abilities[1] = new Ability();
            piece_stats[3].abilities[1].ability_name = "Block";
            piece_stats[3].abilities[1].ability_max = 8;
            piece_stats[3].abilities[1].ability_min = 6;
            piece_stats[3].abilities[1].defense = true;

            piece_stats[3].abilities[2] = new Ability();
            piece_stats[3].abilities[2].ability_name = "Charge";
            piece_stats[3].abilities[2].ability_max = 12;
            piece_stats[3].abilities[2].ability_min = 8;
            piece_stats[3].abilities[2].defensive_strike = true;
        }

        // Bishop Data
        {
            piece_stats[4] = new PieceUnit();
            piece_stats[4].name = "Bishop";
            piece_stats[4].health = 22;

            piece_stats[4].abilities[0] = new Ability();
            piece_stats[4].abilities[0].ability_name = "Strike";
            piece_stats[4].abilities[0].ability_max = 15;
            piece_stats[4].abilities[0].ability_min = 12;

            piece_stats[4].abilities[1] = new Ability();
            piece_stats[4].abilities[1].ability_name = "Block";
            piece_stats[4].abilities[1].ability_max = 10;
            piece_stats[4].abilities[1].ability_min = 7;
            piece_stats[4].abilities[1].defense = true;

            piece_stats[4].abilities[2] = new Ability();
            piece_stats[4].abilities[2].ability_name = "Smite";
            piece_stats[4].abilities[2].ability_max = 25;
            piece_stats[4].abilities[2].ability_min = 10;
            piece_stats[4].abilities[2].weakened = true;
        }

        // Queen Data
        {
            piece_stats[5] = new PieceUnit();
            piece_stats[5].name = "Queen";
            piece_stats[5].health = 40;

            piece_stats[5].abilities[0] = new Ability();
            piece_stats[5].abilities[0].ability_name = "Strike";
            piece_stats[5].abilities[0].ability_max = 12;
            piece_stats[5].abilities[0].ability_min = 10;

            piece_stats[5].abilities[1] = new Ability();
            piece_stats[5].abilities[1].ability_name = "Block";
            piece_stats[5].abilities[1].ability_max = 5;
            piece_stats[5].abilities[1].ability_min = 2;
            piece_stats[5].abilities[1].defense = true;

            piece_stats[5].abilities[2] = new Ability();
            piece_stats[5].abilities[2].ability_name = "Impale";
            piece_stats[5].abilities[2].ability_max = 25;
            piece_stats[5].abilities[2].ability_min = 15;
            piece_stats[5].abilities[2].weakened = true;

            piece_stats[5].abilities[3] = new Ability();
            piece_stats[5].abilities[3].ability_name = "Iron Curtain";
            piece_stats[5].abilities[3].ability_max = 30;
            piece_stats[5].abilities[3].ability_min = 30;
            piece_stats[5].abilities[3].defense = true;
        }

        // King Data
        {
            piece_stats[6] = new PieceUnit();
            piece_stats[6].name = "King";
            piece_stats[6].health = 25;

            piece_stats[6].abilities[0] = new Ability();
            piece_stats[6].abilities[0].ability_name = "Strike";
            piece_stats[6].abilities[0].ability_max = 12;
            piece_stats[6].abilities[0].ability_min = 8;

            piece_stats[6].abilities[1] = new Ability();
            piece_stats[6].abilities[1].ability_name = "Block";
            piece_stats[6].abilities[1].ability_max = 6;
            piece_stats[6].abilities[1].ability_min = 4;
            piece_stats[6].abilities[1].defense = true;

            piece_stats[6].abilities[2] = new Ability();
            piece_stats[6].abilities[2].ability_name = "Royal Smack";
            piece_stats[6].abilities[2].ability_max = 40;
            piece_stats[6].abilities[2].ability_min = 25;
            piece_stats[6].abilities[2].weakened = true;
        }
    }
}
