using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;
using BepuPhysics.Constraints;
using bepuphysics2_for_nelalen.Characters;
using BepuUtilities;
using BepuUtilities.Memory;
using Com.Nelalen.GameObject;
using DemoContentLoader;
using DemoRenderer;
using DemoRenderer.UI;
using DemoUtilities;
using OpenTK.Input;
using static Com.Nelalen.GameObject.Character;

namespace bepuphysics2_for_nelalen
{
    internal class Bepu
    {
        //<unitId,Character>
        private SortedDictionary<int, Character> characters = new SortedDictionary<int, Character>();
        CharacterControllers characterCTs;

        public Simulation Simulation { get; internal set; }
        public BufferPool BufferPool { get; private set; }
        public SimpleThreadDispatcher ThreadDispatcher { get; internal set; }

        internal void Initialize(ContentArchive content, Camera camera)
        {
            ThreadDispatcher = new SimpleThreadDispatcher(Environment.ProcessorCount);
            BufferPool = new BufferPool();
            camera.Position = new Vector3(20, 10, 20);
            camera.Yaw = MathF.PI;
            camera.Pitch = 0;
            characterCTs = new CharacterControllers(BufferPool);

            var collider = new BodyProperty<Collider>();
            Simulation = Simulation.Create(BufferPool, new CharacterNarrowphaseCallbacks(characterCTs), new DemoPoseIntegratorCallbacks(new Vector3(0, -10, 0)));
            //Simulation = Simulation.Create(BufferPool, new MolCallbacks { Collider = collider, Characters= characterCTs }, new DemoPoseIntegratorCallbacks(new System.Numerics.Vector3(0, -10, 0)));
            CreateCharacter(characterCTs,new Vector3(0, 2, -4),1);
            CreateCharacter(characterCTs,new Vector3(0, 3, -2),2);
            Simulation.Statics.Add(new StaticDescription(new Vector3(0, 0, 0), new CollidableDescription(Simulation.Shapes.Add(new Box(200, 1, 200)), 0.1f)));

        }
        Character character;
        int bodyHandle;
        void CreateCharacter(CharacterControllers characterCTs, Vector3 position,int unitId)
        {
            var shape = new Capsule(0.5f, 1);
            var shapeIndex = Simulation.Shapes.Add(shape);
            bodyHandle = Simulation.Bodies.Add(BodyDescription.CreateDynamic(position, new BodyInertia { InverseMass = 1f / 1f  }, new CollidableDescription(shapeIndex, 0.1f), new BodyActivityDescription(shape.Radius * 0.02f)));
            character = new Character(unitId, "test",this,position);
            character.SetCharacterInput(characterCTs, bodyHandle,Simulation);
            characters.Add(character.UnitId,character);
            //Console.WriteLine($"characters[0]: {characters}");

            //character = new CharacterInput(characters, position, new Capsule(0.5f, 1), 0.1f, 1, 20, 100, 6, 4, MathF.PI * 0.4f);
        }
        static Key MoveForward = Key.W;
        static Key MoveBackward = Key.S;
        static Key MoveRight = Key.D;
        static Key MoveLeft = Key.A;
        int i = 0;
        internal void Update(Window window, Camera camera, Input input, float dt)
        {
            i++;
            if (i % 60 == 0) {
                var Velo = character.VelocityInBepu.Linear;
                Console.WriteLine($"x: {Velo.X} y: {Velo.Y} z: {Velo.Z}");
            }
            Vector2 movementDirection1 = new Vector2(1, 0);
            Vector2 movementDirection2 = new Vector2(0, 1);
            /*
            if (input.IsDown(MoveForward))
            {
                movementDirection = new Vector2(0, 1);
            }
            if (input.IsDown(MoveBackward))
            {
                movementDirection += new Vector2(0, -1);
            }
            if (input.IsDown(MoveLeft))
            {
                movementDirection += new Vector2(-1, 0);
            }
            if (input.IsDown(MoveRight))
            {
                movementDirection += new Vector2(1, 0);
            }*/
            const float simulationDt = 1 / 60f;
            characters.TryGetValue(1,out character);
            character.CharacterInputt.UpdateCharacterGoals(movementDirection1, character.MoveSpeed, camera, simulationDt);

            characters.TryGetValue(2, out character);
            character.CharacterInputt.UpdateCharacterGoals(movementDirection2, character.MoveSpeed, camera, simulationDt);
            Simulation.Timestep(1 / 60f, ThreadDispatcher);
        }

        public void Render(Renderer renderer, Camera camera, Input input, TextBuilder text, Font font)
        {
            float textHeight = 16;
            var position = new Vector2(32, renderer.Surface.Resolution.Y - textHeight * 9);
            //renderer.TextBatcher.Write(text.Clear().Append("Toggle character: C"), position, textHeight, new Vector3(1), font);
            position.Y += textHeight * 1.2f;
            character.CharacterInputt.RenderControls(position, textHeight, renderer.TextBatcher, text, font);
            character.CharacterInputt.UpdateCameraPosition(camera);
        }

        protected void OnDispose()
        {

        }
        bool disposed;
        internal void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                OnDispose();
                Simulation.Dispose();
                BufferPool.Clear();
                ThreadDispatcher.Dispose();
            }
        }
    }

    struct MolCallbacks : INarrowPhaseCallbacks
    {
        public BodyProperty<Collider> Collider;
        public CharacterControllers Characters;
        public void Initialize(Simulation simulation)
        {
            Collider.Initialize(simulation.Bodies);
            Characters.Initialize(simulation);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b)
        {
            //It's impossible for two statics to collide, and pairs are sorted such that bodies always come before statics.
            /*if (b.Mobility != CollidableMobility.Static)
            {
                return SubgroupCollisionFilter.AllowCollision(Properties[a.Handle].Filter, Properties[b.Handle].Filter);
            }*/

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
        {
            return true;
        }

        /*[MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe void CreateMaterial(CollidablePair pair, out PairMaterialProperties pairMaterial)
        {
            pairMaterial.FrictionCoefficient = Properties[pair.A.Handle].Friction;
            if (pair.B.Mobility != CollidableMobility.Static)
            {
                //If two bodies collide, just average the friction.
                pairMaterial.FrictionCoefficient = (pairMaterial.FrictionCoefficient + Properties[pair.B.Handle].Friction) * 0.5f;
            }
            pairMaterial.MaximumRecoveryVelocity = 2f;
            pairMaterial.SpringSettings = new SpringSettings(30, 1);
        }*/
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void GetMaterial(out PairMaterialProperties pairMaterial)
        {
            pairMaterial = new PairMaterialProperties { FrictionCoefficient = 1, MaximumRecoveryVelocity = 2, SpringSettings = new SpringSettings(30, 1) };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool ConfigureContactManifold(int workerIndex, CollidablePair pair, NonconvexContactManifold* manifold, out PairMaterialProperties pairMaterial)
        {
            GetMaterial(out pairMaterial);
            //Console.WriteLine("ConfigureContactManifold1");
            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool ConfigureContactManifold(int workerIndex, CollidablePair pair, ConvexContactManifold* manifold, out PairMaterialProperties pairMaterial)
        {
            GetMaterial(out pairMaterial);
            //Console.WriteLine("ConfigureContactManifold2");
            //Console.WriteLine(pair.A.Handle);
            //Console.WriteLine(pair.B.Handle);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ConvexContactManifold* manifold)
        {
            //Console.WriteLine("ConfigureContactManifold3");

            return true;
        }

        public void Dispose()
        {
            Collider.Dispose();
            
        }
    }

    struct CharacterNarrowphaseCallbacks : INarrowPhaseCallbacks
    {
        public CharacterControllers Characters;

        public CharacterNarrowphaseCallbacks(CharacterControllers characters)
        {
            Characters = characters;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(int workerIndex, CollidableReference a, CollidableReference b)
        {
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllowContactGeneration(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB)
        {
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void GetMaterial(out PairMaterialProperties pairMaterial)
        {
            pairMaterial = new PairMaterialProperties { FrictionCoefficient = 1, MaximumRecoveryVelocity = 2, SpringSettings = new SpringSettings(30, 1) };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool ConfigureContactManifold(int workerIndex, CollidablePair pair, ConvexContactManifold* manifold, out PairMaterialProperties pairMaterial)
        {
            GetMaterial(out pairMaterial);
            Characters.TryReportContacts(pair, ref *manifold, workerIndex, ref pairMaterial);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool ConfigureContactManifold(int workerIndex, CollidablePair pair, NonconvexContactManifold* manifold, out PairMaterialProperties pairMaterial)
        {
            GetMaterial(out pairMaterial);
            Characters.TryReportContacts(pair, ref *manifold, workerIndex, ref pairMaterial);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe bool ConfigureContactManifold(int workerIndex, CollidablePair pair, int childIndexA, int childIndexB, ConvexContactManifold* manifold)
        {
            return true;
        }

        public void Dispose()
        {
            Characters.Dispose();
        }

        public void Initialize(Simulation simulation)
        {
            Characters.Initialize(simulation);
        }
    }
    public struct DemoPoseIntegratorCallbacks : IPoseIntegratorCallbacks
    {
        public Vector3 Gravity;
        public float LinearDamping;
        public float AngularDamping;
        Vector3 gravityDt;
        float linearDampingDt;
        float angularDampingDt;

        public AngularIntegrationMode AngularIntegrationMode => AngularIntegrationMode.Nonconserving;

        public DemoPoseIntegratorCallbacks(Vector3 gravity, float linearDamping = .03f, float angularDamping = .03f) : this()
        {
            Gravity = gravity;
            LinearDamping = linearDamping;
            AngularDamping = angularDamping;
        }

        public void PrepareForIntegration(float dt)
        {
            //No reason to recalculate gravity * dt for every body; just cache it ahead of time.
            gravityDt = Gravity * dt;
            //Since this doesn't use per-body damping, we can precalculate everything.
            linearDampingDt = MathF.Pow(MathHelper.Clamp(1 - LinearDamping, 0, 1), dt);
            angularDampingDt = MathF.Pow(MathHelper.Clamp(1 - AngularDamping, 0, 1), dt);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void IntegrateVelocity(int bodyIndex, in RigidPose pose, in BodyInertia localInertia, int workerIndex, ref BodyVelocity velocity)
        {
            //Note that we avoid accelerating kinematics. Kinematics are any body with an inverse mass of zero (so a mass of ~infinity). No force can move them.
            if (localInertia.InverseMass > 0)
            {
                velocity.Linear = (velocity.Linear + gravityDt) * linearDampingDt;
                velocity.Angular = velocity.Angular * angularDampingDt;
            }
            //Implementation sidenote: Why aren't kinematics all bundled together separately from dynamics to avoid this per-body condition?
            //Because kinematics can have a velocity- that is what distinguishes them from a static object. The solver must read velocities of all bodies involved in a constraint.
            //Under ideal conditions, those bodies will be near in memory to increase the chances of a cache hit. If kinematics are separately bundled, the the number of cache
            //misses necessarily increases. Slowing down the solver in order to speed up the pose integrator is a really, really bad trade, especially when the benefit is a few ALU ops.

            //Note that you CAN technically modify the pose in IntegrateVelocity. The PoseIntegrator has already integrated the previous velocity into the position, but you can modify it again
            //if you really wanted to.
            //This is also a handy spot to implement things like position dependent gravity or per-body damping.
        }

    }
}
