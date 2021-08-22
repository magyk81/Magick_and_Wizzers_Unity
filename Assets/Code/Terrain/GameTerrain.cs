using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameTerrain
{
    protected GameTerrain(){}

    #region static terrains
    public static readonly TerrainBase NORMAL = new TerrainBase(
        TerrainBase.Type.NORMAL);
    public static readonly TerrainBase GROVE = new TerrainBase(
        TerrainBase.Type.GROVE);
    public static readonly TerrainBase LAKE = new TerrainBase(
        TerrainBase.Type.LAKE);
    public static readonly TerrainBase PEAK = new TerrainBase(
        TerrainBase.Type.PEAK);
    public static readonly TerrainBase MEADOW = new TerrainBase(
        TerrainBase.Type.MEADOW);
    public static readonly TerrainBase FEN = new TerrainBase(
        TerrainBase.Type.FEN);
    public static readonly TerrainBase WASTE = new TerrainBase(
        TerrainBase.Type.WASTE);
    public static readonly TerrainBase TOON = new TerrainBase(
        TerrainBase.Type.TOON);
    public static readonly TerrainCombo WOOD = new TerrainCombo(
        "Wood", new TerrainBase.Type[] {TerrainBase.Type.GROVE},
        TerrainCombo.Adjacency.TWIN);
    public static readonly TerrainCombo GREAT_LAKE = new TerrainCombo(
        "Great Lake", new TerrainBase.Type[] {TerrainBase.Type.LAKE},
        TerrainCombo.Adjacency.TWIN);
    public static readonly TerrainCombo TWIN_PEAKS = new TerrainCombo(
        "Twin Peaks", new TerrainBase.Type[] {TerrainBase.Type.PEAK},
        TerrainCombo.Adjacency.TWIN);
    public static readonly TerrainCombo FIELD = new TerrainCombo(
        "Field", new TerrainBase.Type[] {TerrainBase.Type.MEADOW},
        TerrainCombo.Adjacency.TWIN);
    public static readonly TerrainCombo BOG = new TerrainCombo(
        "Bog", new TerrainBase.Type[] {TerrainBase.Type.FEN},
        TerrainCombo.Adjacency.TWIN);
    public static readonly TerrainCombo BARREN = new TerrainCombo(
        "Barren", new TerrainBase.Type[] {TerrainBase.Type.WASTE},
        TerrainCombo.Adjacency.TWIN);
    public static readonly TerrainCombo FOREST = new TerrainCombo(
        "Forest", new TerrainBase.Type[] {TerrainBase.Type.GROVE},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo SEA = new TerrainCombo(
        "Sea", new TerrainBase.Type[] {TerrainBase.Type.LAKE},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo MOUNTAIN_RANGE = new TerrainCombo(
        "Mountain Range", new TerrainBase.Type[] {TerrainBase.Type.PEAK},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo PLAIN = new TerrainCombo(
        "Plain", new TerrainBase.Type[] {TerrainBase.Type.MEADOW},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo DESERT = new TerrainCombo(
        "Desert", new TerrainBase.Type[] {TerrainBase.Type.WASTE},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo GLACIER = new TerrainCombo(
        "Glacier", new TerrainBase.Type[] {
            TerrainBase.Type.LAKE, TerrainBase.Type.MEADOW},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo CRUSH = new TerrainCombo(
        "Crush", new TerrainBase.Type[] {
            TerrainBase.Type.LAKE, TerrainBase.Type.FEN},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo VOLCANO = new TerrainCombo(
        "Volcano", new TerrainBase.Type[] {
            TerrainBase.Type.PEAK, TerrainBase.Type.FEN},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo QUESTION_1 = new TerrainCombo(
        "???", new TerrainBase.Type[] {
            TerrainBase.Type.PEAK, TerrainBase.Type.GROVE},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo SAVANNAH = new TerrainCombo(
        "Savannah", new TerrainBase.Type[] {
            TerrainBase.Type.GROVE, TerrainBase.Type.MEADOW},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo MESA = new TerrainCombo(
        "Mesa", new TerrainBase.Type[] {
            TerrainBase.Type.MEADOW, TerrainBase.Type.PEAK},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo QUESTION_2 = new TerrainCombo(
        "???", new TerrainBase.Type[] {
            TerrainBase.Type.GROVE, TerrainBase.Type.FEN},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo GEYSER = new TerrainCombo(
        "Geyser", new TerrainBase.Type[] {
            TerrainBase.Type.LAKE, TerrainBase.Type.PEAK},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo QUESTION_3 = new TerrainCombo(
        "???", new TerrainBase.Type[] {
            TerrainBase.Type.LAKE, TerrainBase.Type.PEAK},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo TROPIC = new TerrainCombo(
        "Tropic", new TerrainBase.Type[] {
            TerrainBase.Type.GROVE, TerrainBase.Type.LAKE},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo QUESTION_4 = new TerrainCombo(
        "???", new TerrainBase.Type[] {
            TerrainBase.Type.MEADOW, TerrainBase.Type.GROVE,
            TerrainBase.Type.LAKE},
        TerrainCombo.Adjacency.FLEXIBLE);
    public static readonly TerrainCombo QUESTION_5 = new TerrainCombo(
        "???", new TerrainBase.Type[] {
            TerrainBase.Type.MEADOW, TerrainBase.Type.GROVE,
            TerrainBase.Type.PEAK},
        TerrainCombo.Adjacency.FLEXIBLE);
    public static readonly TerrainCombo PILLAR = new TerrainCombo(
        "Pillar", new TerrainBase.Type[] {
            TerrainBase.Type.GROVE, TerrainBase.Type.PEAK,
            TerrainBase.Type.LAKE},
        TerrainCombo.Adjacency.FLEXIBLE);
    public static readonly TerrainCombo GRAVEYARD = new TerrainCombo(
        "Graveyard", new TerrainBase.Type[] {
            TerrainBase.Type.FEN, TerrainBase.Type.PEAK,
            TerrainBase.Type.LAKE, TerrainBase.Type.WASTE},
        TerrainCombo.Adjacency.FLEXIBLE);
    public static readonly TerrainCombo ÆTHER = new TerrainCombo(
        "Æther", new TerrainBase.Type[] {
            TerrainBase.Type.MEADOW, TerrainBase.Type.FEN,
            TerrainBase.Type.LAKE},
        TerrainCombo.Adjacency.FLEXIBLE);
    public static readonly TerrainCombo BAD = new TerrainCombo(
        "Bad", new TerrainBase.Type[] {
            TerrainBase.Type.MEADOW, TerrainBase.Type.FEN,
            TerrainBase.Type.PEAK, TerrainBase.Type.WASTE},
        TerrainCombo.Adjacency.FLEXIBLE);
    public static readonly TerrainCombo QUESTION_6 = new TerrainCombo(
        "???", new TerrainBase.Type[] {
            TerrainBase.Type.GROVE, TerrainBase.Type.PEAK,
            TerrainBase.Type.LAKE},
        TerrainCombo.Adjacency.FLEXIBLE);
    public static readonly TerrainCombo QUESTION_7 = new TerrainCombo(
        "???", new TerrainBase.Type[] {
            TerrainBase.Type.GROVE, TerrainBase.Type.FEN,
            TerrainBase.Type.MEADOW},
        TerrainCombo.Adjacency.FLEXIBLE);
    public static readonly TerrainCombo JUNGLE = new TerrainCombo(
        "Jungle", new TerrainBase.Type[] {
            TerrainBase.Type.GROVE, TerrainBase.Type.FEN,
            TerrainBase.Type.LAKE},
        TerrainCombo.Adjacency.FLEXIBLE);
    public static readonly TerrainCombo TUNDRA = new TerrainCombo(
        "Tundra", new TerrainBase.Type[] {
            TerrainBase.Type.MEADOW, TerrainBase.Type.PEAK,
            TerrainBase.Type.LAKE, TerrainBase.Type.WASTE},
        TerrainCombo.Adjacency.FLEXIBLE);
    public static readonly TerrainCombo EARTH_POWER = new TerrainCombo(
        "Earth Power", new TerrainBase.Type[] {
            TerrainBase.Type.GROVE, TerrainBase.Type.TOON},
        TerrainCombo.Adjacency.RIGID);
    public static readonly TerrainCombo OUIJA = new TerrainCombo(
        "Ouija", new TerrainBase.Type[] {
            TerrainBase.Type.PEAK, TerrainBase.Type.FEN,
            TerrainBase.Type.TOON},
        TerrainCombo.Adjacency.FLEXIBLE);
    public static readonly TerrainCombo GREEN = new TerrainCombo(
        "Green", new TerrainBase.Type[] {
            TerrainBase.Type.NORMAL, TerrainBase.Type.FEN,
            TerrainBase.Type.TOON, TerrainBase.Type.LAKE},
        TerrainCombo.Adjacency.RIGID);
    #endregion
}
