using System;
using static Area51HW.Building51;

namespace Area51HW
{
    internal class Building51
    {
        public Random random = new Random();
        public enum Floor
        {
            G,
            S,
            T1,
            T2
        }

        internal class Agent
        {
            public enum SecurityLevel
            {
                Confidential,
                Secret,
                TopSecret
            }

            public string Name { get; set; }
            public SecurityLevel SecurityCredentials { get; set; }
            public Floor CurrentFloor { get; set; }
            public bool InsideElevator { get; set; }
            public Floor ElevatorDestination { get; set; }
            public Floor desiredDestination { get; set; }

            public void CallElevator(Agent callerAgent , Elevator elevator)
            {
                if (ShouldCallElevator())
                {
                    Thread.Sleep(50);
                    ElevatorDestination = GetRandomFloor();
                    callerAgent.desiredDestination = ElevatorDestination;
                    Console.WriteLine($"{Name} called the elevator to floor {CurrentFloor} with destination {ElevatorDestination}.");
                    elevator.isNotInUse = false;
                    elevator.MoveToFloorAfterOutsideCall(callerAgent.CurrentFloor, callerAgent, elevator);
                }
            }

            public bool ShouldCallElevator()
            {
                return new Random().NextDouble() < 0.08; // don't turn this up, unless you want people stampeding over eachother
                                                         // and the elevator lines giving up and the whole building exploding
                                                         // letting all the aliens loose from where they will reign havoc on all
                                                         // nations just because you turned up this value
            }

            private Floor GetRandomFloor()
            {
                Random random = new Random();
                return (Floor)random.Next(Enum.GetNames(typeof(Floor)).Length);
            }
        }

        internal class Elevator
        {
            private Floor currentFloor;
            
            private object lockObject = new object();
            public bool isNotInUse = true;
            

            public Elevator()
            {
                
                currentFloor = Floor.G;
            }

            //public void CallElevator(Agent callingAgent, Elevator elevator, Floor toAgentFloor)
            //{
              //  elevator.isNotInUse = true;
                
                //    Console.WriteLine($"{callingAgent.Name} called the elevator to floor {callingAgent.CurrentFloor}.");
                  //  MoveToFloorAfterOutsideCall( toAgentFloor, callingAgent, elevator);
                
            //}

            public void MoveToFloorAfterOutsideCall(Floor floor, Agent callerAgent, Elevator elevator)
            {
                Thread.Sleep(1000 * Math.Abs((int)floor - (int)currentFloor)); // elevator travel time according to floors traveled
                elevator.currentFloor = callerAgent.desiredDestination;
                callerAgent.CurrentFloor = callerAgent.desiredDestination;

                
                    Console.WriteLine($"Elevator arrived at floor {floor}.");
                    callerAgent.InsideElevator = true;
                    OpenDoors(callerAgent, elevator);
                
            }

            public void MoveToFloor(Floor floor, Agent elevatorAgent, Elevator elevator)
            {
                Thread.Sleep(1000 * Math.Abs((int)floor - (int)currentFloor));
                currentFloor = floor;

                
                    Console.WriteLine($"Elevator arrived at floor {floor}.");
                    elevatorAgent.InsideElevator = true;
                    OpenDoors(elevatorAgent, elevator);
                
            }

            public void OpenDoors(Agent movingAgent, Elevator elevator)
            {
                lock (lockObject)
                {
                    if (movingAgent.SecurityCredentials >= GetRequiredSecurityLevel(currentFloor))
                    {
                        
                        Console.WriteLine($"{movingAgent.Name} exited the elevator on floor {currentFloor}.");
                        movingAgent.InsideElevator = false;
                        movingAgent.CurrentFloor = currentFloor; 
                        elevator.isNotInUse= true;
                    }
                    else
                    {
                        Console.WriteLine($"{movingAgent.Name} couldn't exit on floor {currentFloor} due to insufficient security credentials.");
                        PanicMode(movingAgent, elevator);
                    }
                    
                }
            }
            public Random random = new Random();

            private Floor GetRandomFloor()
            {
                Thread.Sleep(30);
                return (Floor)new Random().Next(Enum.GetValues(typeof(Floor)).Length);
            }
            public void PanicMode(Agent agent, Elevator elevator)
            {
                bool isPanicing = true;
                while(isPanicing)
                {
                    if(new Random().NextDouble() < 0.2) // spamming the buttons in panic
                    {
                        isPanicing = false;
                    }
                    else { Thread.Sleep(random.Next(1000, 2000)); Console.WriteLine($"{agent.Name} is struggling to hit an elevator button."); }
                }
                Floor destinationFloor = GetRandomFloor();
                

                
                    agent.ElevatorDestination = destinationFloor;
                    Console.WriteLine($"{agent.Name} entered panic mode and called the elevator to floor {destinationFloor}.");
                

                MoveToFloor(destinationFloor, agent, elevator);

            }

            private Dictionary<Floor, Agent.SecurityLevel> requiredSecurityLevels = new Dictionary<Floor, Agent.SecurityLevel>
            {
                { Floor.G, Agent.SecurityLevel.Confidential },
                { Floor.S, Agent.SecurityLevel.Secret },
                { Floor.T1, Agent.SecurityLevel.TopSecret },
                { Floor.T2, Agent.SecurityLevel.TopSecret }
            };

            private Agent.SecurityLevel GetRequiredSecurityLevel(Floor floor)
            {
                return requiredSecurityLevels.TryGetValue(floor, out var securityLevel) ? securityLevel : Agent.SecurityLevel.Confidential;
            }

        }

        private static List<Agent> agents;

        private static void Main()
        {
            agents = GenerateAgents();

            Elevator elevator = new Elevator();

            foreach (Agent agent in agents)
            {
                Thread agentThread = new Thread(() => AgentOperation(agent, elevator));
                agentThread.Start();
            }

            //Thread elevatorThread = new Thread(() => ElevatorOperation(elevator));
            //elevatorThread.Start();

            //elevatorThread.Join();
        }

        private static List<Agent> GenerateAgents()
        {
            List<Agent> agents = new List<Agent>();

            for (int i = 1; i <= 51; i++)
            {
                agents.Add(new Agent
                {
                    Name = $"Agent {i}",
                    SecurityCredentials = GetRandomSecurityLevel(),
                    CurrentFloor = Floor.G,
                    InsideElevator = false,
                    
                });
            }

            return agents;
        }

        private static Agent.SecurityLevel GetRandomSecurityLevel()
        {
            Array values = Enum.GetValues(typeof(Agent.SecurityLevel));
            return (Agent.SecurityLevel)values.GetValue(new Random().Next(values.Length));
        }

        public static void AgentOperation(Agent agent, Elevator elevator)
        {
            Random random = new Random();
            while (true)
            {
                if(agent.ShouldCallElevator() && elevator.isNotInUse)
                {
                    agent.CallElevator( agent, elevator);
                }
                Thread.Sleep(random.Next(2000, 5000));
            }

        }

        

        //public static void ElevatorOperation(Elevator elevator)
        //{
            
        //}
    }
}
