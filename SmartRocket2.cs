using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SmartRocket2 : MonoBehaviour
{
    public static int lifespan = 250;
    int count = 0;
    int generation = 0;
    int bestGen;
    int bestTime;
    float maxfit;
    float fitness;
    float random;
    float dist;
    float tempDist;
 
    Vector2 force;
    Vector2 tempVector;

    Rocket bestRocket;
    Rocket bestRocketLastGen;
    Rocket[] Child = new Rocket[40];
    Rocket[] rocketClass = new Rocket[40];
    List<Rocket> matingPool = new List<Rocket>();

    GameObject bestRocketObj;
    GameObject bestRocketLastGenObj;
    GameObject[] rockets = new GameObject[40];
    GameObject[] childRockets = new GameObject[40];

    public Text lifespanUI;
    public Text generationUI;
    public Text highestFitnessUI;
    public Button BestRocketUI;
    public Slider targetFramerate;

    public int popSize = 40;
    public GameObject rocketPreFab;
    public GameObject ChildGO;
    public GameObject firePoint;
    public Transform emptyPos;
    public GameObject target;
    public float maxForce;

    bool firstRound = true;
    bool runBestRocket = false;
    void Start()
    {
        highestFitnessUI.GetComponent<Text>().enabled = false;
        
        bestRocketObj = Instantiate(rocketPreFab, emptyPos.transform.position, emptyPos.transform.rotation);
        bestRocketObj.AddComponent<Rocket>();
        bestRocket = bestRocketObj.GetComponent<Rocket>();
        bestRocket.setRocket(bestRocketObj);
        bestRocket.setPos(emptyPos.transform.position.x, emptyPos.transform.position.y);
        bestRocket.GetComponent<SpriteRenderer>().enabled = false;
        bestRocket.name = "Best Rocket";

        bestRocketLastGenObj = Instantiate(rocketPreFab);
        bestRocketLastGenObj.AddComponent<Rocket>();
        bestRocketLastGen = bestRocketLastGenObj.GetComponent<Rocket>();
        bestRocketLastGen.GetComponent<SpriteRenderer>().enabled = false;
        for (int i = 0; i < popSize; i++) {
            childRockets[i] = Instantiate(ChildGO);
            childRockets[i].AddComponent<Rocket>();
            Child[i] = childRockets[i].GetComponent<Rocket>();

            rockets[i] = Instantiate(rocketPreFab, firePoint.transform.position, firePoint.transform.rotation);
            rockets[i].AddComponent<Rocket>();
            rocketClass[i] = rockets[i].GetComponent<Rocket>();
            
            rocketClass[i].setRocket(rockets[i]);
            rocketClass[i].setPos(firePoint.transform.position.x, firePoint.transform.position.y);
            rocketClass[i].name = "Rocket " + (i+1);
            rocketClass[i].GetComponent<SpriteRenderer>().enabled = true;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (runBestRocket) {
            BestRun();
        }
        else { 
            NormalRun();
        }
    }
    public Vector2 RandomVector() {
        random = UnityEngine.Random.Range(0f, 260f);
        return new Vector2(Mathf.Cos(random) * maxForce, Mathf.Sin(random) * maxForce);
    }
    public void NormalRun() {
        if (generation == 0) {
            BestRocketUI.interactable = false;
        }
        else {
            BestRocketUI.interactable = true;
        }
        if (count == 0) {
            for (int i = 0; i < popSize; i++) {
                rocketClass[i].setPos(firePoint.transform.position.x, firePoint.transform.position.y);
                rocketClass[i].setVel(0, 0);
                rocketClass[i].setCrash(false);
                rocketClass[i].setComp(false);
                rocketClass[i].setDist(75);
                rocketClass[i].setTime();
                rocketClass[i].GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        if (count < lifespan) {
            for (int i = 0; i < popSize; i++) {
                if (rocketClass[i].getComp() == false) {
                    rocketClass[i].addTime();
                }
                if (rocketClass[i].getCrash() == false && rocketClass[i].getComp() == false) {
                    if (firstRound == true)
                    {
                        force = RandomVector();
                        rocketClass[i].setDNA(force, count);
                        rocketClass[i].applyForce(force);
                        dist = (target.transform.position - new Vector3(rocketClass[i].getPos().x, rocketClass[i].getPos().y)).magnitude;
                        if (dist < rocketClass[i].getDist()) {
                            rocketClass[i].setDist(dist);
                        }
                        rocketClass[i].UpdateRocket(dist);
                    }
                    else {
                        tempVector = rocketClass[i].mutation(Child[i].getDNA(count));
                        //if (i < ((popSize*2)/3)) {
                        //    tempVector = rocketClass[i].mutation(Child[i].getDNA(count));
                        //}
                        //else {
                        //    tempVector = RandomVector();
                        //}
                        rocketClass[i].setDNA(tempVector, count);
                        rocketClass[i].applyForce(tempVector);
                        dist = (target.transform.position - new Vector3(rocketClass[i].getPos().x, rocketClass[i].getPos().y)).magnitude;
                        if (dist < rocketClass[i].getDist())
                        {
                            rocketClass[i].setDist(dist);
                        }
                        rocketClass[i].UpdateRocket(dist);
                    }
                }
            }
            count++;
        }
        if (count == lifespan) {
            maxfit = 0.000001f;
            for (int i = 0; i < popSize; i++) {
                fitness = rocketClass[i].calcFitness();
                if (fitness > maxfit) {
                    maxfit = fitness;
                }
            }
            for (int i = 0; i < popSize; i++) {
                rocketClass[i].setFitness(rocketClass[i].getFitness() / maxfit);
            }
            MateRockets();
            UpdateText();
            bestRocketLastGen.setFitness(0);
            count = 0;
            generation++;
            firstRound = false;
        }
        lifespanUI.text = "Lifespan = " + (count + 1);
        generationUI.text = "Current Generation = " + (generation + 1);
    }
    public void MateRockets() { 
        matingPool.Clear();
        for (int i = 0; i < popSize; i++) {
            float n;
            if (rocketClass[i].getCrash()) {
                n = ((((1/(rocketClass[i].getDist()+2))+0.5f)*5000) - (rocketClass[i].getTime() * 10))/6.5f;
            }
            else if (rocketClass[i].getComp()) {
                n = (((rocketClass[i].getFitness() + 0.5f) * 5000) - (rocketClass[i].getTime() * 10)) / 20;
            }
            else {
                n = ((((1/(rocketClass[i].getDist()+2))+0.5f)*5000) - (rocketClass[i].getTime() * 10)) / 6;
            }
            rocketClass[i].setFitness(n);
            for (int j = 0; j < n; j++) {
                matingPool.Add(rocketClass[i]);
            }
            //save best rocket if current rocketclass fitness was higher than current best rocket
            if (rocketClass[i].getFitness() > bestRocket.getFitness()) {
                //set bestRocket DNA equal to current rocketclass DNA
                for (int j = 0; j < lifespan; j++)
                {
                    bestRocket.setDNA(rocketClass[i].getDNA(j), j);
                }
                bestRocket.setFitness(rocketClass[i].getFitness());
                bestRocket.name = rocketClass[i].name;
                bestRocket.setTime(rocketClass[i].getTime());
                bestRocket.setCrash(rocketClass[i].getCrash());
                bestRocket.setComp(rocketClass[i].getComp());
                bestRocket.setDist(rocketClass[i].getDist());
                bestRocket.setPos(emptyPos.transform.position.x, emptyPos.transform.position.y);
                bestGen = generation;
            }
            if (rocketClass[i].getFitness() > bestRocketLastGen.getFitness()) {
                for (int j = 0; j < lifespan; j++) {
                    bestRocketLastGen.setDNA(rocketClass[i].getDNA(j), j);
                }
                bestRocketLastGen.setFitness(rocketClass[i].getFitness());
                bestRocketLastGen.name = rocketClass[i].name;
                bestRocketLastGen.setTime(rocketClass[i].getTime());
                bestRocketLastGen.setCrash(rocketClass[i].getCrash());
                bestRocketLastGen.setComp(rocketClass[i].getComp());
                bestRocketLastGen.setDist(rocketClass[i].getDist());
                bestRocketLastGen.setPos(emptyPos.transform.position.x, emptyPos.transform.position.y);
            }
        }
        for (int i = 0; i < popSize; i++) {
            int indexA = UnityEngine.Random.Range(0, matingPool.Count);
            int indexB = UnityEngine.Random.Range(0, matingPool.Count);
            Rocket parentA = matingPool[indexA];
            Rocket parentB = matingPool[indexB];
            if (bestRocket.getFitness() > 110) {
                Child[i].setDNA(parentA.crossover(bestRocket));
            }
            else {
                Child[i].setDNA(parentA.crossover(parentB));
            }
        }
    }
    public void UpdateText() { 
        //Print Statements for Best Rocket from the last generation
        //Print Statements for Best Rocket from all generations
        if (bestRocket.getCrash()) {
            highestFitnessUI.text = bestRocket.name + " from Generation #" + (bestGen + 1) + "\nI crashed " + bestRocket.getDist() + " ly away";
        }
        else if (bestRocket.getComp()) {
            highestFitnessUI.text = bestRocket.name + " from Generation #" + (bestGen + 1) + "\nI took " + bestRocket.getTime() + " frames to reach the target";
        }
        else {
            highestFitnessUI.text = bestRocket.name + " from Generation #" + (bestGen + 1) + "\nIt took " + bestRocket.getTime() + " frames to get " + bestRocket.getDist() + " ly away";
        }
    }
    public void BestRun() {
        if (count == 0) {
            FrameRateScript.target = 30;
            generationUI.enabled = false;
            lifespanUI.enabled = false;
            for (int i = 0; i < popSize; i++) {
                rocketClass[i].GetComponent<SpriteRenderer>().enabled = false;
            }
            bestRocket.GetComponent<SpriteRenderer>().enabled = true;
            bestRocket.setPos(firePoint.transform.position.x, firePoint.transform.position.y);
            bestRocket.setDist(75);
        }
        if (count < bestTime) {
            if (bestRocket.getComp() == false) {
                bestRocket.addTime();
            }
            if (bestRocket.getCrash() == false && bestRocket.getComp() == false) {
                bestRocket.applyForce(bestRocket.getDNA(count));
                tempDist = (target.transform.position - new Vector3(bestRocket.getPos().x, bestRocket.getPos().y)).magnitude;
                if(tempDist < bestRocket.getDist()) { 
                    bestRocket.setDist(tempDist);
                }
                bestRocket.UpdateRocket(tempDist);
            }
            count++;
            lifespanUI.text = "Lifespan = " + (count + 1);
        }
        else {
            FrameRateScript.target = (int)targetFramerate.value;
            runBestRocket = false;
            bestRocket.GetComponent<SpriteRenderer>().enabled = false;
            BestRocketUI.interactable = true;
            count = 0;
            highestFitnessUI.enabled = false;
            generationUI.enabled = true;
            lifespanUI.enabled = true;
        }
    }

    public void ResetButton() {
        for (int i = 0; i < popSize; i++) {
            rocketClass[i].resetAll();
        }
        
        runBestRocket = false;
        bestRocket.resetAll();
        bestRocket.GetComponent<SpriteRenderer>().enabled = false;
        bestRocketLastGen.resetAll();
        FrameRateScript.target = (int)targetFramerate.value;

        firstRound = true;
        generation = 0;
        count = 0;
        
        BestRocketUI.interactable = true;
        lifespanUI.text = "Lifespan = ";
        generationUI.text = "Current Generation = ";
        highestFitnessUI.text = "Highest Fitness from all Generations: ";
    }
    public void BestRocketRunButton() {
        BestRocketUI.interactable = false;
        runBestRocket = true;
        count = 0;
        bestRocket.setVel(0f, 0f);
        bestTime = bestRocket.getTime();
        bestRocket.setTime();
        bestRocket.setComp(false);
        bestRocket.setCrash(false);
        highestFitnessUI.GetComponent<Text>().enabled = true;
    }
}