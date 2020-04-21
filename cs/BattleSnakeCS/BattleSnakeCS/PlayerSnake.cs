using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Diagnostics;

namespace BattleSnakeCS
{
    public class PlayerSnake : Snake
    {
        public struct Personalisation
        {
            public string mColor;
            public string mHeadType;
            public string mTailType;

            public Personalisation(string color, string headType, string tailType)
            {
                mColor = color;  //"#ff00ff";
                mHeadType = headType;  //"bendr";
                mTailType = tailType;  //"pixel";
            }
        }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            None
        }

        private enum Axis
        {
            X,
            Y,
            None
        }

        private Personalisation mPersonalisation;

        private bool mHasTarget = false; 
        private Point mTarget = new Point();

        private Direction mDirectionOfTravel = Direction.Up;
        private Axis mTargetMoveAxis = Axis.None; 

        public PlayerSnake()
        {
            // Defaults for personalisation
            mPersonalisation = new Personalisation("#ff00ff", "bendr", "pixel"); 
        }

        public PlayerSnake(PlayerSnake other, JObject payload)
        {
            InitialisePlayerSnake(payload);
            mPersonalisation = other.GetPersonalisation(); 
        }

        public Personalisation GetPersonalisation()
        {
            return mPersonalisation; 
        }

        public void SetPersonalisation(JObject payload)
        {
            mPersonalisation.mColor = (string)payload["color"];
            mPersonalisation.mHeadType = (string)payload["headType"];
            mPersonalisation.mTailType = (string)payload["tailType"];
        }

        public void InitialisePlayerSnake(JObject payload)
        {
            mBody = new List<Point>();

            mID = (string)payload["you"]["id"];
            mName = (string)payload["you"]["name"];
            mHealth = (int)payload["you"]["health"];

            DeserialisePlayerSnake(payload);
        }

        public void DeserialisePlayerSnake(JObject payload)
        {
            // Update health
            mHealth = (int)payload["you"]["health"];

            // Erase old body and update with new coords
            mBody.Clear(); 

            JArray snakeBodyJSON = (JArray)payload["you"]["body"];
            foreach (var bodyCoord in snakeBodyJSON.Children())
            {
                int x = (int)bodyCoord["x"];
                int y = (int)bodyCoord["y"];
                mBody.Add(new Point(x, y));
            }
        }

        public string GetSnakePersonalisationJSON()
        {
            JObject personalisationContentJSON =
                new JObject(
                    new JProperty("color", mPersonalisation.mColor),
                    new JProperty("headType", mPersonalisation.mHeadType),
                    new JProperty("tailType", mPersonalisation.mTailType));

            return personalisationContentJSON.ToString(Newtonsoft.Json.Formatting.None); 
        }

        public void WritePersonalisationToFile(string filename)
        {
            string path = Path.Combine(Environment.CurrentDirectory, filename);
            File.WriteAllText(path, GetSnakePersonalisationJSON());
        }

        public void ReadPersonalisationFromFile(string filename)
        {
            string path = Path.Combine(Environment.CurrentDirectory, filename);
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                SetPersonalisation(JObject.Parse(json));
            }
        }

        //=====================================
        // Snake logic follows
        //=====================================
        public Direction GetNextMove(Board gameBoard)
        {
            Point head = mBody[0];
            //Debug.WriteLine("Head Location: X:" + head.X + ", Y:" + head.Y); 

            if (mHasTarget == false)
            {
                mTarget = GetNearestTarget(gameBoard.GetFood());
                Debug.WriteLine("Nearest Target: X:" + mTarget.X + ", Y:" + mTarget.Y);
                mHasTarget = true; 
            }

            if (head.Equals(mTarget))
            {
                // Reached the target. Pick a new one immediately!
                mTarget = GetNearestTarget(gameBoard.GetFood());
                Debug.WriteLine("Nearest Target: X:" + mTarget.X + ", Y:" + mTarget.Y);
            }

            mDirectionOfTravel = SteerTowardsTarget(mTarget);

            if (WillCollideWall(gameBoard))
            {
                mDirectionOfTravel = SteerAwayFromWall(mDirectionOfTravel, gameBoard); 

                return mDirectionOfTravel; 
            }
            else
            {
                // No change
                return mDirectionOfTravel; 
            }
        }

        /// <summary>
        /// Checks if the snake will collide with a wall if it
        /// continues in the current direction of travel. 
        /// </summary>
        /// <param name="gameBoard"></param>
        /// <returns></returns>
        private bool WillCollideWall(Board gameBoard)
        {
            if (mBody.Count > 0)
            {
                Point head = mBody[0];

                if (mDirectionOfTravel == Direction.Up)
                {
                    if (head.Y - 1 < 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false; 
                    }
                }
                else if (mDirectionOfTravel == Direction.Down)
                {
                    if (head.Y + 1 == gameBoard.GetBoardSize().Y)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (mDirectionOfTravel == Direction.Left)
                {
                    if (head.X - 1 < 0)
                    {
                        return true; 
                    }
                    else
                    {
                        return false; 
                    }
                }
                else if (mDirectionOfTravel == Direction.Right)
                {
                    if (head.X + 1 == gameBoard.GetBoardSize().X)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            
            // TODO: Really should return an exception because the body doesnt exist 
            return false; 
        }

        /// <summary>
        /// Make a move that will steer away from a wall
        /// </summary>
        /// <returns></returns>
        private Direction SteerAwayFromWall(Direction directionOfTravel, Board gameBoard)
        {
            Point head = mBody[0];
            Direction steeringDirection = Direction.None; 

            // Steer away from wall. Right now it doesn't matter in which direction, just stay away from it. 
            if (directionOfTravel == Direction.Up || directionOfTravel == Direction.Down)
            {
                if (head.X == 0)
                {
                    steeringDirection = Direction.Right;
                }
                else if (head.X == gameBoard.GetBoardSize().X)
                {
                    steeringDirection = Direction.Left;
                }
                else
                {
                    // TODO: This is just a hard coded move, we would really do something else
                    steeringDirection = Direction.Left;
                }
            }
            else if (directionOfTravel == Direction.Left || directionOfTravel == Direction.Right)
            {
                if (head.Y == 0)
                {
                    steeringDirection = Direction.Down;
                }
                else if (head.Y == gameBoard.GetBoardSize().Y)
                {
                    steeringDirection = Direction.Up;
                }
                else
                {
                    // TODO: This is just a hard coded move, we would really do something else
                    steeringDirection = Direction.Up;
                }
            }

            return steeringDirection; 
        }

        /// <summary>
        /// Checks all food on the board and returns the one that is the closest. 
        /// The distance is calculated using Manhattan distance method. 
        /// </summary>
        /// <returns></returns>
        private Point GetNearestTarget(List<Point> food)
        {
            Point head = mBody[0];
            Point target = new Point(); 
            int shortestDist = int.MaxValue; 

            foreach (Point f in food)
            {
                // Calculate manhattan distance
                int deltaX = Math.Abs(f.X - head.X);
                int deltaY = Math.Abs(f.Y - head.Y); 
                
                if (deltaX + deltaY < shortestDist)
                {
                    shortestDist = deltaX + deltaY; 
                    target = f; 
                }
            }

            return target; 
        }

        private Direction SteerTowardsTarget(Point target)
        {
            Point head = mBody[0];
            Direction steeringDirection = Direction.None;

            /*
             * First steer towards the target so that player moves
             * either vertically or horizontally until inline with
             * the target on either X or Y axis, then steer towards
             * target on the other axis. 
             */
            if (mDirectionOfTravel == Direction.Up || mDirectionOfTravel == Direction.Down)
            {
                // Steer left or right toward target
                if (mTarget.X < head.X)
                {
                    steeringDirection = Direction.Left;
                }
                else if (mTarget.X > head.X)
                {
                    steeringDirection = Direction.Right;
                }
                else
                {
                    // Player head now inline vertically with target, switch target movement axis
                    mTargetMoveAxis = Axis.Y; 
                    if (mTarget.Y < head.Y)
                    {
                        steeringDirection = Direction.Up;
                    }
                    else
                    {
                        steeringDirection = Direction.Down; 
                    }
                }
            }
            else
            {
                // Steer up or down toward target
                if (mTarget.Y < head.Y)
                {
                    steeringDirection = Direction.Up;
                }
                else if (mTarget.Y > head.Y)
                {
                    steeringDirection = Direction.Down;
                }
                else
                {
                    // Player head now inlne horizontally with target, switch target movement axis
                    mTargetMoveAxis = Axis.X;

                    if (mTarget.X < head.X)
                    {
                        steeringDirection = Direction.Left;
                    }
                    else
                    {
                        steeringDirection = Direction.Right;
                    }
                }
            }

            if (WillCollideBody(steeringDirection))
            {
                Debug.Write("Want to turn: " + steeringDirection + ", will collide!"); 
                steeringDirection = OppositeDirection(steeringDirection); 
            }

            return steeringDirection; 
        }

        /// <summary>
        /// Check wether collision with own body will occur
        /// as result of a steering choice
        /// </summary>
        /// <param name="steeringDirection"></param>
        /// <returns></returns>
        private bool WillCollideBody(Direction steeringDirection)
        {
            Point head = mBody[0];
            Point checkSpace = new Point(); 

            switch (steeringDirection)
            {
                case Direction.Left:
                    checkSpace = new Point(head.X - 1, head.Y);
                    break;
                case Direction.Right:
                    checkSpace = new Point(head.X + 1, head.Y);
                    break;
                case Direction.Up:
                    checkSpace = new Point(head.X, head.Y - 1);
                    break;
                case Direction.Down:
                    checkSpace = new Point(head.X, head.Y + 1);
                    break;
                default:
                    checkSpace = new Point(-1, -1);
                    break;
            };

            foreach (Point p in mBody)
            {
                if (checkSpace.Equals(p))
                {
                    Debug.WriteLine("Check Space: " + checkSpace + ", Point: " + p); 
                    // The check space is part of the snake body.
                    return true;
                }
            }

            return false; 
        }

        private Direction OppositeDirection(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                default:
                    return Direction.None; 
            }
        }
    }
}
