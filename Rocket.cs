using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Rocket : MonoBehaviour{
    public GameObject firePoint;
    public GameObject target;

    Vector2 pos = new Vector2();
    Vector2 vel = new Vector2();
    Vector2 acc = new Vector2();

    Vector2[] DNA;
    Vector2[] newDNA;


    bool completed = false;
    bool crashed = false;

    float dist;
    float fitness;
    int time = 0;
    float rand1, rand2;

    GameObject rocket;
    void Start() {
        DNA = new Vector2[SmartRocket2.lifespan];
        newDNA = new Vector2[SmartRocket2.lifespan];
    }
    public void resetAll() {
        completed = false;
        crashed = false;

        dist = 0;
        fitness = 0;
        time = 0;
    }
    public int getTime() {
        return time;
    }
    public void setTime() {
        time = 0;
    }
    public void setTime(int tempTime)
    {
        time = tempTime;
    }
    public void addTime()
    {
        time += 1;
    }
    public Vector2 getNewDNA(int index) {
        return newDNA[index];
    }
    public void setNewDNA(Vector2 gene, int index) {
        newDNA[index] = gene;
    }
    public Vector2 getDNA(int index) {
        return DNA[index];
    }
    public Vector2[] getDNA() {
        return DNA;
    }
    public void setDNA(Vector2 gene, int index) {
        DNA[index] = gene;
    }
    public void setDNA(Vector2[] genes)
    {
        DNA = genes;
    }
    public float getFitness() {
        return fitness;
    }
    public void setFitness(float tempFit) {
        fitness = tempFit;
    }
    public void setDist(float tempDist) {
        dist = tempDist;
    }
    public float getDist()
    {
        return dist;
    }
    public Vector2 getPos()
    {
        return pos;
    }
    public void setPos(float x, float y) {
        pos = new Vector2(x, y);
    }
    public Vector2 getVel()
    {
        return vel;
    }
    public void setVel(float x, float y)
    {
        vel = new Vector2(x, y);
    }
    public bool getComp() {
        return completed;
    }
    public void setComp(bool tf)
    {
        completed = tf;
    }
    public bool getCrash()
    {
        return crashed;
    }
    public void setCrash(bool tf) {
        crashed = tf;
    }
    public GameObject getRocket()
    {
        return rocket;
    }
    public void setRocket(GameObject tempRocket) {
        rocket = tempRocket;
    }
    public Vector2[] newCrossover(Rocket partner1, Rocket partner2)
    {
        Array.Clear(newDNA, 0, newDNA.Length);
        for (int i = 0; i < newDNA.Length; i++)
        {
            float pickParent = UnityEngine.Random.Range(0f, 100f);
            if (pickParent < 20) {
                newDNA[i] = DNA[i];
            }
            else if(pickParent>=20 && pickParent < 60) {
                newDNA[i] = partner1.getDNA(i);
            }
            else {
                newDNA[i] = partner2.getDNA(i);
            }
        }
        return newDNA;
    }

    public Vector2[] crossover(Rocket partner)
    {
        Array.Clear(newDNA, 0, newDNA.Length);
        for (int i = 0; i < newDNA.Length; i++)
        {
            int pickParent = UnityEngine.Random.Range(0, 100);
            if (pickParent < 50)
            {
                newDNA[i] = DNA[i];
            }
            else
            {
                newDNA[i] = partner.getDNA(i);
            }
        }
        return newDNA;
    }
    public Vector2 mutation(Vector2 currentDNA) {
        Vector2 randomDNA = currentDNA;
        rand1 = UnityEngine.Random.Range(0f, 1f);
        if ((Mathf.Abs(randomDNA.x)<0.000001f && Mathf.Abs(randomDNA.y) < 0.000001f) || rand1 < .04f) {
            rand2 = UnityEngine.Random.Range(0f, 260f);
            randomDNA = new Vector2(Mathf.Cos(rand2) * .02f, Mathf.Sin(rand2) * .02f);
        }
        return randomDNA;
    }
    public void applyForce(Vector2 force) {
        acc += force;
    }
    public void UpdateRocket(float dist) {
        if (dist < 1.9f) {
            completed = true;
            rocket.GetComponent<SpriteRenderer>().enabled = false;
        }
        vel += acc;
        pos += vel;
        acc *= 0;
        Vector3 dir = new Vector3(pos.x, pos.y) - rocket.transform.position;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rocket.transform.position = pos;
        rocket.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward) * Quaternion.Euler(0f, 0f, 1f);
    }
    public float calcFitness() {
        fitness = 1/(10*dist);
        return fitness;
    }
}
