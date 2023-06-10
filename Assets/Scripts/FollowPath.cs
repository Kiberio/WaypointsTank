using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class FollowPath : MonoBehaviour
{
     //Declarando as variáveis
    Transform goal;
    float speed = 15.0f;
    float accuracy = 1.0f;
    float rotSpeed = 2.0f;
    public GameObject wpManager;
    GameObject[] wps;
    GameObject currentNode;
    int currentWP = 0;
    Graph g;
    private NavMeshAgent _agent;
    private Ray _ray;
    private RaycastHit _hit;
    private Camera _camera;
    private static readonly int ground = 1 << 6;


    //Inicia pegando a lista de waypoints do WPManager, gera o graph e manda para o node 0 para uma nova varredura
    void Start()
    {
        wps = wpManager.GetComponent<WPManager>().waypoints;
        g = wpManager.GetComponent<WPManager>().graph;
        currentNode = wps[0];
        _agent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
    }

//Método que envia para a area do heliponto, sendo o wp 1 o local
    public void GoToHeli()
    {
        g.AStar(currentNode, wps[1]);
        currentWP = 0;
    }

//Método que envia para a area da usina, sendo o wp 9 o local
    public void GoToUsine()
    {
        g.AStar(currentNode, wps[9]);
        currentWP = 0;
    }

//Método que envia para a area da ruina, sendo o wp 7 o local
    public void GoToRuin()
    {
        g.AStar(currentNode, wps[7]);
        currentWP = 0;
    }

//Método que usei para um teste, mas nao esta sendo utilizado
    public void GoToMountain()
    {
        g.AStar(currentNode, wps[12]);
        currentWP = 0;
    }

//Checa após os updates, se o player apertou o mouse ou nao, se sim, define o destino do tank até onde o mouse estava.
    void LateUpdate()
    {
        if(Input.GetMouseButtonDown(0))
        {
            _ray = _camera.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(_ray, out _hit, 1000f, ground))
            {
                _agent.destination = _hit.point;
            }
        }

//Area de atualizacao do algoritmo AStar, onde ele checa os waypoints, distancia, tambem usa transform para movimentar o tank e rotacionar
        if (g.getPathLength() == 0 || currentWP == g.getPathLength())
            return;

        currentNode = g.getPathPoint(currentWP);
        if (Vector3.Distance(
            g.getPathPoint(currentWP).transform.position,
            transform.position) < accuracy)
        {
            currentWP++;
        }

        if (currentWP < g.getPathLength())
        {
            goal = g.getPathPoint(currentWP).transform;
            Vector3 lookAtGoal = new Vector3(goal.position.x,
                this.transform.position.y,
                goal.position.z);
            Vector3 direction = lookAtGoal - this.transform.position;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                Quaternion.LookRotation(direction),
                Time.deltaTime * rotSpeed);
//Linha de codigo adicionada, para o tank se movimentar utilizando o canvas
            transform.position = Vector3.MoveTowards(transform.position, goal.position, speed * Time.deltaTime);
        }
    }
}
