using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainBACKUP : MonoBehaviour
{


    [SerializeField]
    public int selected = 1;

    private float elapsed = 0.0f;
    public Camera cam;
    public NavMeshAgent agent;
    //public GameObject clock;
    public TMPro.TextMeshPro clock;
    public Animator animator;
    public AnimationClip animC;
    private Animation anim = new Animation();
    private float _secondsPersecond = 0.0f;
    //private float _totalGameSeconds = 35997.6f; //10h=35997.6f;
    private float _totalGameSeconds = 86397.6f; //10h=35997.6f;
    //------------------- Bathroom ------------------//
    private bool inBathroom = false;
    private float _secondsSinceBathroom = 0.0f;
    private float _hoursSinceBathroom = 0.0f;
    private float _daysSinceBathroom = 0.0f;


    //------------------- OutOfHouse -----------------//
    private float probReturn = 0.01f;
    private bool _OutOfHouse = false;
    private float _secondsSinceExit = 0.0f;

    private Dictionary<string, float> probabilities = new Dictionary<string, float> {
        {"Salon",0.1666666667f},
        {"Dormitorio",0.1666666667f},
        {"Baño",0.1666666667f},
        {"Cocina",0.1666666667f},
        {"Entrada",0.1666666667f},
        {"Exit",0.1666666667f}
    };
    private Dictionary<string, float> adjProbabilities = new Dictionary<string, float> {
        {"Salon",0.1666666667f},
        {"Dormitorio",0.1666666667f},
        {"Baño",0.1666666667f},
        {"Cocina",0.1666666667f},
        {"Entrada",0.1666666667f},
        {"Exit",0.1666666667f}
    };
    private Dictionary<string, float> realProbabilities = new Dictionary<string, float> {
        {"Salon",0.1666666667f},
        {"Dormitorio",0.1666666667f},
        {"Baño",0.1666666667f},
        {"Cocina",0.1666666667f},
        {"Entrada",0.1666666667f},
        {"Exit",0.1666666667f}
    };
    private bool probChanged = false;

    private string cadena = "";

    float CalcularDistanciaObjeto(string name)
    {
        Vector3 vectorToTarget = GameObject.Find(name).gameObject.transform.position - agent.gameObject.transform.position;
        vectorToTarget.y = 0;
        float distanceToTarget = vectorToTarget.magnitude;
        return distanceToTarget;

    }


    void Start()
    {
        // clock = GameObject.Find("Clock");
        elapsed = 0.0f;
        _secondsPersecond = 120;
        _totalGameSeconds += _secondsPersecond * Time.deltaTime;
        //Debug.Log(_totalGameSeconds);
        float Temp = 0.0f;
        foreach (KeyValuePair<string, float> entry in adjProbabilities)
        {
            Temp += entry.Value;
            realProbabilities[entry.Key] = Temp;
            //Debug.Log(entry.Key + ":" + realProbabilities[entry.Key]);
        }
        agent.updateRotation = false;
        //StartCoroutine(toVideo());
        ///StartCoroutine(MovementCoroutine());
        //StartCoroutine(ToBed());
        //StartCoroutine(toEat());
        //probabilities["Cocina"] = 0.5f;
        //probChanged = true;
    }
    void Update()
    {
        elapsed += Time.deltaTime;
        int days = (int)_totalGameSeconds / 86400;
        int hours = (int)(_totalGameSeconds / 3600) - days * 24;
        int minutes = (int)(_totalGameSeconds / 60) - hours * 60 - days * 24 * 60;
        int seconds = (int)_totalGameSeconds - minutes * 60 - hours * 3600 - days * 24 * 3600;

        // Debug.Log(hours+":"+minutes+":"+seconds);
        //Debug.Log(clock.transform.position);
        //clock.GetComponent<TMPro.TextMeshProUGUI>().text = hours + ":" + minutes + ":" + seconds;
        cadena = hours.ToString("00");
        //clock.SetText("<color=#000000>"+cadena[0]+"</color>"+cadena[1]);
        _totalGameSeconds += _secondsPersecond * Time.deltaTime;

        /**
        if(Input.GetMouseButtonDown(0)){
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit)){
                agent.SetDestination(hit.point);
            }
        }
           
        //animator.Play("Sit",0);
        */
        if ((hours < 23 && hours >= 2) && probabilities["Dormitorio"] != 0.1666666667f)
        {
            //Debug.Log("Entro en revert probabilidades");
            probabilities["Dormitorio"] = 0.1666666667f;
            probChanged = true;
        }
        else if ((hours >= 23 || hours < 2) && probabilities["Dormitorio"] != 0.5f)
        {
            //Debug.Log("Hours: " + hours);
            //Debug.Log("Entro en probabilidades");
            probabilities["Dormitorio"] = 0.5f;
            probChanged = true;
        }

        if ((hours < 15 && hours >= 14))
        {
            if (probabilities["Cocina"] != 0.5f)
            {
                Debug.Log("Probabilidad 0.5");
                probabilities["Cocina"] = 0.5f;
                probChanged = true;
            }

        }
        else if ((hours < 22 && hours >= 21))
        {
            if (probabilities["Cocina"] != 0.5f)
            {
                Debug.Log("Probabilidad 0.5");
                probabilities["Cocina"] = 0.5f;
                probChanged = true;
            }

        }
        else if ((hours < 10 && hours >= 9))
        {
            if (probabilities["Cocina"] != 0.5f)
            {
                Debug.Log("Probabilidad 0.5");
                probabilities["Cocina"] = 0.5f;
                probChanged = true;
            }

        }
        else if (probabilities["Cocina"] != 0.1666666667f)
        {
            Debug.Log("Probabilidad default");
            probabilities["Cocina"] = 0.1666666667f;
            probChanged = true;
        }

        if (!inBathroom)
        {
            _secondsSinceBathroom += _secondsPersecond * Time.deltaTime;
            _daysSinceBathroom = (int)_secondsSinceBathroom / 86400;
            _hoursSinceBathroom = (int)(_secondsSinceBathroom / 3600) - days * 24;
            if (_hoursSinceBathroom < 8)
            {
                //Ecuación de la recta y=0.04761904761x + 0.1666666667
                probabilities["Baño"] = 0.04761904761f * _hoursSinceBathroom + 0.1666666667f;
                probChanged = true;
            }
        }
        else
        {
            _secondsSinceBathroom = 0.0f;
        }

        if (_OutOfHouse)
        {
            _secondsSinceExit += _secondsPersecond * Time.deltaTime;
            if (_secondsSinceExit < 10800)
            {
                //Ecuación de la recta y=0.00004537037x + 0.01
                probReturn = 0.00004537037f * _secondsSinceExit + 0.01f;
            }

        }
        else
        {
            if (_secondsSinceExit > 0)
            {
                _secondsSinceExit -= _secondsPersecond * Time.deltaTime;
                //Ecuación de la recta y=0.00004537037x + 0.01
                probReturn = 0.00004537037f * _secondsSinceExit + 0.01f;
            }
        }


        if ((hours < 13 && hours >= 10))
        {
            if (probabilities["Exit"] != 0.5f)
            {
                probabilities["Exit"] = 0.5f;
                probChanged = true;
            }
        }
        else if (hours > 20 || hours < 9)
        {
            if (probabilities["Exit"] != 0.0f)
            {
                probabilities["Exit"] = 0.0f;
                probChanged = true;
            }
        }
        else if (probabilities["Exit"] != 0.1666666667f)
        {
            probabilities["Exit"] = 0.1666666667f;
            probChanged = true;
        }




        if (probChanged)
        {
            //Debug.Log("ProbChanged false");
            probChanged = false;
            //StopCoroutine(MovementCoroutine());
            StartCoroutine(AdjustProbabilities());

        }



    }

    public IEnumerator AdjustProbabilities()
    {
        float Total = 0.0f;
        float Temp = 0.0f;
        foreach (KeyValuePair<string, float> entry in probabilities)
        {
            Total += entry.Value;
        }
        foreach (KeyValuePair<string, float> entry in probabilities)
        {
            adjProbabilities[entry.Key] = entry.Value / Total;
            //Debug.Log(adjProbabilities[entry.Key]);
        }
        foreach (KeyValuePair<string, float> entry in adjProbabilities)
        {
            Temp += entry.Value;
            realProbabilities[entry.Key] = Temp;
            //Debug.Log(entry.Key+":"+realProbabilities[entry.Key]);
        }

        //StartCoroutine(MovementCoroutine());


        yield return null;
    }

    public IEnumerator MovementCoroutine()
    {
        bool leaving = false;
        float nRandom = 0.0f;
        while (!leaving)
        {
            if (inBathroom)
                inBathroom = false;

            nRandom = Random.value;
            //Debug.Log(nRandom);
            if (nRandom < realProbabilities["Salon"])
            {
                agent.SetDestination(GameObject.Find("Salon").gameObject.transform.position);
                while (CalcularDistanciaObjeto("Salon") != 0)
                {
                    //Debug.Log("A Salon: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Salon").gameObject.transform.position));
                    //animator.SetFloat("Forward",0.4f, 0.1f, Time.deltaTime);
                    yield return null;
                }
                //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
                if (Random.value < 0.15)
                {
                    agent.enabled = false;
                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(11.23f, 3.6f, -32.71f);
                    yield return new WaitForSeconds(1);

                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(11.53f, 3.443815f, -32f);
                    agent.enabled = true;
                }
                yield return new WaitForSeconds(2);
            }
            else if (nRandom < realProbabilities["Dormitorio"])
            {
                agent.SetDestination(GameObject.Find("Dormitorio").gameObject.transform.position);

                while (CalcularDistanciaObjeto("Dormitorio") != 0)
                {
                    //Debug.Log("A Dormitorio: "+Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Dormitorio").gameObject.transform.position));
                    //animator.SetFloat("Forward", 0.4f, 0.1f, Time.deltaTime);
                    yield return null;
                }
                if (probabilities["Dormitorio"] == 0.5f)
                {
                    agent.SetDestination(GameObject.Find("bed1").gameObject.transform.position);

                    while (CalcularDistanciaObjeto("bed1") != 0)
                    {
                        yield return null;
                    }
                    agent.enabled = false;
                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(-90.0f, 90.0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(8.6f, 3.8f, -34.22f);

                    yield return new WaitForSeconds(240);
                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(0f, 0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(8.6f, 3.443815f, -34.22f);
                    agent.enabled = true;
                }
                else
                {
                    if (Random.value < 0.15)
                    {
                        agent.enabled = false;
                        GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                        GameObject.Find("Ethan").transform.position = new Vector3(8.35f, 3.6f, -32.82f);
                        yield return new WaitForSeconds(1);

                        GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                        GameObject.Find("Ethan").transform.position = new Vector3(8.22f, 3.443815f, -32.41f);
                        agent.enabled = true;
                    }
                    yield return new WaitForSeconds(2);
                }
                //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
            }
            else if (nRandom < realProbabilities["Baño"])
            {
                agent.SetDestination(GameObject.Find("Bano").gameObject.transform.position);

                while (CalcularDistanciaObjeto("Bano") != 0)
                {
                    //animator.SetFloat("Forward", 0.4f, 0.1f, Time.deltaTime);
                    //Debug.Log("A Baño: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Bano").gameObject.transform.position));
                    yield return null;
                }
                //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
                inBathroom = true;
                if (Random.value < 0.15)
                {
                    agent.enabled = false;
                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(7.39f, 3.6f, -30.25f);
                    yield return new WaitForSeconds(1);

                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(7.54f, 3.443815f, -29.69f);
                    agent.enabled = true;
                }
                yield return new WaitForSeconds(2);
            }
            else if (nRandom < realProbabilities["Cocina"])
            {
                agent.SetDestination(GameObject.Find("Cocina").gameObject.transform.position);

                while (CalcularDistanciaObjeto("Cocina") != 0)
                {
                    //Debug.Log("A Cocina: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Cocina").gameObject.transform.position));
                    //animator.SetFloat("Forward", 0.4f, 0.1f, Time.deltaTime);
                    yield return null;
                }
                if (probabilities["Cocina"] == 0.5f)
                {
                    agent.SetDestination(GameObject.Find("Chair").gameObject.transform.position);
                    while (CalcularDistanciaObjeto("Chair") != 0)
                    {
                        yield return null;
                    }
                    agent.enabled = false;
                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(11.11f, 4.4f, -34.45f);
                    animator.Play("Sit", 0);

                    yield return new WaitForSeconds(22.5f);
                    animator.Play("Grounded", 0);
                    GameObject.Find("Ethan").transform.position = new Vector3(11.11f, 3.443815f, -34.45f);

                    agent.enabled = true;
                }
                else
                {
                    if (Random.value < 0.15)
                    {
                        agent.enabled = false;
                        GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(90.0f, -45.0f, 0.0f);
                        GameObject.Find("Ethan").transform.position = new Vector3(12.43f, 3.6f, -30.06f);
                        yield return new WaitForSeconds(1);

                        GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                        GameObject.Find("Ethan").transform.position = new Vector3(11.92f, 3.443815f, -29.46f);
                        agent.enabled = true;
                    }
                    yield return new WaitForSeconds(2);
                }


                //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
            }
            else if (nRandom < realProbabilities["Entrada"])
            {
                agent.SetDestination(GameObject.Find("Entry").gameObject.transform.position);

                while (CalcularDistanciaObjeto("Entry") != 0)
                {
                    //Debug.Log("A Entrada: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Entry").gameObject.transform.position));
                    //animator.SetFloat("Forward", 0.4f, 0.1f, Time.deltaTime);
                    yield return null;
                }
                //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
                if (Random.value < 0.15)
                {
                    agent.enabled = false;
                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(9.93f, 3.6f, -30.54f);
                    yield return new WaitForSeconds(1);

                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(10.02f, 3.443815f, -29.85f);
                    agent.enabled = true;
                }
                yield return new WaitForSeconds(2);
            }
            else if (nRandom < realProbabilities["Exit"])
            {

                agent.SetDestination(GameObject.Find("Exit").gameObject.transform.position);

                while (CalcularDistanciaObjeto("Exit") != 0)
                {
                    //Debug.Log("A Salida: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Exit").gameObject.transform.position));
                    yield return null;
                }


                leaving = true;

            }

        }

        transform.Find("EthanBody").gameObject.SetActive(false);
        transform.Find("EthanGlasses").gameObject.SetActive(false);
        transform.Find("EthanSkeleton").gameObject.SetActive(false);

        Debug.Log("Saliendo");
        yield return null;
        StartCoroutine(OutOfHouse());
    }

















    /** ------------------------------------ MOVEMENTCOROUTINE OLD-----------------------------------------------------
    public IEnumerator MovementCoroutine() {
        bool leaving=false;
        int nRandom=0;
        while(!leaving){
            nRandom=Random.Range(0,6);
            switch(nRandom){
                case 0:
                         agent.SetDestination(GameObject.Find("Salon").gameObject.transform.position);
                        while(Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Salon").gameObject.transform.position) >=0.09){
                        //Debug.Log("A Salon: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Salon").gameObject.transform.position));
                        //animator.SetFloat("Forward",0.4f, 0.1f, Time.deltaTime);
                        yield return null;
                        }
                    //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
                    break;
                case 1:
                                agent.SetDestination(GameObject.Find("Dormitorio").gameObject.transform.position);
                            
                            while(Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Dormitorio").gameObject.transform.position) >=0.09){
                        //Debug.Log("A Dormitorio: "+Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Dormitorio").gameObject.transform.position));
                        //animator.SetFloat("Forward", 0.4f, 0.1f, Time.deltaTime);
                        yield return null;
                            }
                    //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
                    break;
                case 2:
                        agent.SetDestination(GameObject.Find("Bano").gameObject.transform.position);
            
                        while(Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Bano").gameObject.transform.position) >=0.09){
                        //animator.SetFloat("Forward", 0.4f, 0.1f, Time.deltaTime);
                        //Debug.Log("A Baño: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Bano").gameObject.transform.position));
                        yield return null;
                        }
                    //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
                    break;
                case 3:
                        agent.SetDestination(GameObject.Find("Cocina").gameObject.transform.position);
            
                        while(Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Cocina").gameObject.transform.position) >=0.09){
                        //Debug.Log("A Cocina: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Cocina").gameObject.transform.position));
                        //animator.SetFloat("Forward", 0.4f, 0.1f, Time.deltaTime);
                        yield return null;
                        }
                    //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
                    break;
                case 4:
                            agent.SetDestination(GameObject.Find("Entry").gameObject.transform.position);
                        
                        while(Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Entry").gameObject.transform.position) >=0.09){
                        //Debug.Log("A Entrada: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Entry").gameObject.transform.position));
                        //animator.SetFloat("Forward", 0.4f, 0.1f, Time.deltaTime);
                        yield return null;
                        }
                    //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
                    break;
                case 5:
                        
                         agent.SetDestination(GameObject.Find("Exit").gameObject.transform.position);
                        
                        while(Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Exit").gameObject.transform.position) >=0.09){
                        //Debug.Log("A Salida: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Exit").gameObject.transform.position));
                        yield return null;
                        }
                   
                    
                    leaving = true;
                    break;
            }
        }
        
        GameObject.Find("EthanBody").SetActive(false) ;
        GameObject.Find("EthanGlasses").SetActive(false);
        GameObject.Find("EthanSkeleton").SetActive(false);
        
        Debug.Log("Saliendo");
        yield return null;
        StartCoroutine(OutOfHouse());
    }
    **/
    public IEnumerator ToBed()
    {
        agent.SetDestination(GameObject.Find("bed1").gameObject.transform.position);
        /*
        var vectorToTarget = GameObject.Find("bed1").gameObject.transform.position - agent.gameObject.transform.position;
        vectorToTarget.y = 0;
        var distanceToTarget = vectorToTarget.magnitude;
        */
        while (CalcularDistanciaObjeto("bed1") != 0)
        {
            //Debug.Log("A Cocina: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Cocina").gameObject.transform.position));
            //animator.SetFloat("Forward", 0.4f, 0.1f, Time.deltaTime);
            //vectorToTarget = GameObject.Find("bed1").gameObject.transform.position - agent.gameObject.transform.position;
            //vectorToTarget.y = 0;
            //distanceToTarget = vectorToTarget.magnitude;
            //Debug.Log(distanceToTarget);
            yield return null;
        }
        agent.enabled = false;
        GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(-90.0f, 90.0f, 0.0f);
        GameObject.Find("Ethan").transform.position = new Vector3(8.6f, 3.8f, -34.22f);
        //GameObject.Find("Ethan").transform.rotation.Set(-90f, 90f, 0.0f,0.0f);
        //GameObject.Find("Ethan").transform.position.Set(7.82f, 3.710f, -34.22f);

        yield return null;
    }
    public IEnumerator OutOfHouse()
    {
        _OutOfHouse = true;
        bool comeback = false;
        while (!comeback)
        {
            Debug.Log("ENTRO: " + probReturn);
            if (Random.value < probReturn)
            {
                //Debug.Log("ENTRO2");
                comeback = true;

                transform.Find("EthanBody").gameObject.SetActive(true);
                transform.Find("EthanGlasses").gameObject.SetActive(true);
                transform.Find("EthanSkeleton").gameObject.SetActive(true);
                agent.SetDestination(GameObject.Find("Entry").gameObject.transform.position);

                while (Vector3.Distance(agent.gameObject.transform.position, GameObject.Find("Entry").gameObject.transform.position) >= 0.09)
                {
                    //Debug.Log("A Entrada: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Entry").gameObject.transform.position));
                    //animator.SetFloat("Forward", 0.4f, 0.1f, Time.deltaTime);
                    yield return null;
                }
                _OutOfHouse = false;
                //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
            }
            else
            {

                yield return new WaitForSeconds(2f);

            }
        }
        StartCoroutine(MovementCoroutine());
        yield return null;
    }
    public IEnumerator toEat()
    {
        agent.SetDestination(GameObject.Find("Chair").gameObject.transform.position);
        while (CalcularDistanciaObjeto("Chair") != 0)
        {
            yield return null;
        }
        agent.enabled = false;
        GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        GameObject.Find("Ethan").transform.position = new Vector3(11.11f, 4.4f, -34.45f);
        animator.Play("Sit", 0);
        yield return new WaitForSeconds(500f);
        GameObject.Find("Ethan").transform.position = new Vector3(12f, 3.443815f, -34.45f);
        animator.Play("Grounded", 0);

    }

    public IEnumerator try1()
    {
        int i = 0;
        while (Vector3.Distance(agent.gameObject.transform.position, GameObject.Find("Salon").gameObject.transform.position) >= 0.015)
        {
            Debug.Log("Distancia 2: " + Vector3.Distance(agent.gameObject.transform.position, GameObject.Find("Salon").gameObject.transform.position));
            yield return null;
        }
        yield return null;
    }


    public IEnumerator toVideo()
    {
        bool leaving = false;
        float nRandom = 0.0f;
        while (!leaving)
        {
            if (inBathroom)
                inBathroom = false;

            nRandom = Random.value;
            //Debug.Log(nRandom);
            if (selected == 1)
            {
                agent.SetDestination(GameObject.Find("Salon").gameObject.transform.position);
                while (CalcularDistanciaObjeto("Salon") != 0)
                {
                    //Debug.Log("A Salon: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Salon").gameObject.transform.position));
                    //animator.SetFloat("Forward",0.4f, 0.1f, Time.deltaTime);
                    yield return null;
                }
                //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
                agent.enabled = false;
                GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                GameObject.Find("Ethan").transform.position = new Vector3(11.23f, 3.6f, -32.71f);
                yield return new WaitForSeconds(1);

                GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                GameObject.Find("Ethan").transform.position = new Vector3(11.53f, 3.443815f, -32f);
                agent.enabled = true;
                yield return new WaitForSeconds(2);
            }
            else if (selected == 2)
            {
                agent.SetDestination(GameObject.Find("Dormitorio").gameObject.transform.position);

                while (CalcularDistanciaObjeto("Dormitorio") != 0)
                {
                    //Debug.Log("A Dormitorio: "+Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Dormitorio").gameObject.transform.position));
                    //animator.SetFloat("Forward", 0.4f, 0.1f, Time.deltaTime);
                    yield return null;
                }
                if (probabilities["Dormitorio"] == 0.5f)
                {
                    agent.SetDestination(GameObject.Find("bed1").gameObject.transform.position);

                    while (CalcularDistanciaObjeto("bed1") != 0)
                    {
                        yield return null;
                    }
                    agent.enabled = false;
                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(-90.0f, 90.0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(8.6f, 3.8f, -34.22f);

                    yield return new WaitForSeconds(240);
                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(0f, 0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(8.6f, 3.443815f, -34.22f);
                    agent.enabled = true;
                }
                else
                {
                    agent.enabled = false;
                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(8.35f, 3.6f, -32.82f);
                    yield return new WaitForSeconds(1);

                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(8.22f, 3.443815f, -32.41f);
                    agent.enabled = true;
                    yield return new WaitForSeconds(2);
                }
                //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
            }
            else if (selected == 3)
            {
                agent.SetDestination(GameObject.Find("Bano").gameObject.transform.position);

                while (CalcularDistanciaObjeto("Bano") != 0)
                {
                    //animator.SetFloat("Forward", 0.4f, 0.1f, Time.deltaTime);
                    //Debug.Log("A Baño: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Bano").gameObject.transform.position));
                    yield return null;
                }
                //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
                inBathroom = true;
                agent.enabled = false;
                GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                GameObject.Find("Ethan").transform.position = new Vector3(7.39f, 3.6f, -30.25f);
                yield return new WaitForSeconds(1);

                GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                GameObject.Find("Ethan").transform.position = new Vector3(7.54f, 3.443815f, -29.69f);
                agent.enabled = true;
                yield return new WaitForSeconds(2);
            }
            else if (selected == 4)
            {
                agent.SetDestination(GameObject.Find("Cocina").gameObject.transform.position);

                while (CalcularDistanciaObjeto("Cocina") != 0)
                {
                    //Debug.Log("A Cocina: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Cocina").gameObject.transform.position));
                    //animator.SetFloat("Forward", 0.4f, 0.1f, Time.deltaTime);
                    yield return null;
                }
                if (probabilities["Cocina"] == 0.5f)
                {
                    agent.SetDestination(GameObject.Find("Chair").gameObject.transform.position);
                    while (CalcularDistanciaObjeto("Chair") != 0)
                    {
                        yield return null;
                    }
                    agent.enabled = false;
                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(11.11f, 4.4f, -34.45f);
                    animator.Play("Sit", 0);

                    yield return new WaitForSeconds(22.5f);
                    animator.Play("Grounded", 0);
                    GameObject.Find("Ethan").transform.position = new Vector3(11.11f, 3.443815f, -34.45f);

                    agent.enabled = true;
                }
                else
                {
                    agent.enabled = false;
                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(90.0f, -45.0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(12.43f, 3.6f, -30.06f);
                    yield return new WaitForSeconds(1);

                    GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                    GameObject.Find("Ethan").transform.position = new Vector3(11.92f, 3.443815f, -29.46f);
                    agent.enabled = true;
                    yield return new WaitForSeconds(2);
                }


                //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
            }
            else if (selected == 5)
            {
                agent.SetDestination(GameObject.Find("Entry").gameObject.transform.position);

                while (CalcularDistanciaObjeto("Entry") != 0)
                {
                    //Debug.Log("A Entrada: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Entry").gameObject.transform.position));
                    //animator.SetFloat("Forward", 0.4f, 0.1f, Time.deltaTime);
                    yield return null;
                }
                //animator.SetFloat("Forward", 0.0f, 0.1f, Time.deltaTime);
                agent.enabled = false;
                GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                GameObject.Find("Ethan").transform.position = new Vector3(9.93f, 3.6f, -30.54f);
                yield return new WaitForSeconds(1);

                GameObject.Find("Ethan").transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                GameObject.Find("Ethan").transform.position = new Vector3(10.02f, 3.443815f, -29.85f);
                agent.enabled = true;
                yield return new WaitForSeconds(2);
            }
            else if (selected == 6)
            {

                agent.SetDestination(GameObject.Find("Exit").gameObject.transform.position);

                while (CalcularDistanciaObjeto("Exit") != 0)
                {
                    //Debug.Log("A Salida: " + Vector3.Distance(agent.gameObject.transform.position,GameObject.Find("Exit").gameObject.transform.position));
                    yield return null;
                }


                leaving = true;

            }

        }

        transform.Find("EthanBody").gameObject.SetActive(false);
        transform.Find("EthanGlasses").gameObject.SetActive(false);
        transform.Find("EthanSkeleton").gameObject.SetActive(false);

        Debug.Log("Saliendo");
        yield return null;
        StartCoroutine(OutOfHouse());
    }





}




