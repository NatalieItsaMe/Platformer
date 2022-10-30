using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;
using Platformer.Components;
using System.Collections.Generic;

namespace Platformer
{
    internal class PhysicsSystem : EntityUpdateSystem
    {
        public Vector2 RIGHT = new Vector2(1, 0);
        public Vector2 UP = new Vector2(1, 0);
        public Vector2 Gravity = new Vector2(0, -2);
        public Vector2 TerminalVelocity = new Vector2(12.4f, 12.4f);

        internal const float minGroundNormalY = 0.2f;
        internal const float shellRadius = 1 / 32f;
        protected const float minMoveDistance = 1 / 512f;

        protected RaycastHit[] hitBuffer = new RaycastHit[8];
        protected List<RaycastHit> hitList = new List<RaycastHit>(8);

        private ComponentMapper<Transform2> _transforms;
        private ComponentMapper<Physics> _physics;
        private ComponentMapper<Collider> _colliders;

        public PhysicsSystem() : base(Aspect.All(typeof(Physics), typeof(Transform2), typeof(Collider)))
        {
        }

        public override void Initialize(IComponentMapperService mapperService)
        {
            _transforms = mapperService.GetMapper<Transform2>();
            _physics = mapperService.GetMapper<Physics>();
            _colliders = mapperService.GetMapper<Collider>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach(var entity in ActiveEntities)
            {
                _physics.Get(entity).grounded = false;
                UpdateVelocity(gameTime.GetElapsedSeconds(), entity);
                UpdatePosition(gameTime.GetElapsedSeconds(), entity);
            }
        }

        private void UpdateVelocity(float seconds, int entity)
        {
            var velocity = _physics.Get(entity).Velocity;
            var acceleration = _physics.Get(entity).Acceleration;

            velocity += acceleration * seconds;
            velocity += Gravity * seconds;

            if (velocity.Y > TerminalVelocity.Y)
                velocity.SetY(TerminalVelocity.Y);
            if (velocity.X > TerminalVelocity.X)
                velocity.SetX(TerminalVelocity.X);

            _physics.Get(entity).Velocity = velocity;
        }

        private void UpdatePosition(float seconds, int entity)
        {
            var velocity = _physics.Get(entity).Velocity;

            Movement(entity, RIGHT * velocity.X * seconds, false);
            Movement(entity, UP * velocity.Y * seconds, true);
        }

        void Movement(int entity, Vector2 move, bool yMovement)
        {
            Collider collider = _colliders.Get(entity);
            Physics physics = _physics.Get(entity);
            Transform2 transform = _transforms.Get(entity);

            float delta = move.Length();

            if (delta > minMoveDistance)
            {
                hitBuffer = Collider.Cast(collider, move, delta + shellRadius);
                hitList.Clear();
                for (int i = 0; i < hitBuffer.Length; i++)
                {
                    hitList.Add(hitBuffer[i]);
                }

                for (int i = 0; i < hitList.Count; i++)
                {
                    Vector2 currentNormal = hitList[i].normal;

                    if (collider.bounds.Min.Y < collider.bounds.Max.Y)
                    {
                        transform.Position += move;
                        return;
                    }

                    if (yMovement)
                    {
                        if (currentNormal.Y >= minGroundNormalY) //hit the floor
                        {
                            physics.Velocity.SetY(physics.groundedVelocity);
                            physics.grounded = true;

                            //pulls the object to the ground
                            float distance = hitList[i].distance;
                            if (distance > shellRadius)
                            {
                                distance -= shellRadius;
                                transform.Position -= UP * distance;
                                return;
                            }
                        }
                        if (currentNormal.Y < -minGroundNormalY) //hits head on ceiling
                        {
                            physics.Velocity.SetY(0.5f * physics.Velocity.Y);
                        }
                    }
                    else
                    {
                        if (currentNormal.Y >= minGroundNormalY)
                        {
                            Vector2 groundDirection = new Vector2(currentNormal.Y, -currentNormal.X);
                            move = groundDirection * move.Y / groundDirection.X;

                            //dampens the speed over steeper slopes
                            if (currentNormal.Y <= 0.707)
                                move *= 0.5f;

                            transform.Position += move;
                            return;
                        }
                        else
                        {
                            physics.Velocity.SetX(0);
                        }
                    }

                    float modifiedDistance = hitList[i].distance - shellRadius;
                    delta = modifiedDistance < delta ? modifiedDistance : delta;
                }


            }

            transform.Position += move.NormalizedCopy() * delta;
        }
    }
}
