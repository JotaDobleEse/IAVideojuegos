using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Math;
using WaveEngine.Components.Graphics2D;
using WaveEngine.Components.UI;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Services;
using WaveProject.Characters;
using WaveProject.Steerings.Combined;
using WaveProject.Steerings.Coordinated;
using WaveProject.Steerings.Delegated;
using WaveProject.Steerings.Pathfinding;

namespace WaveProject
{
    public class GameController : Behavior
    {
        Kinematic Mouse;
        Vector2 LastMousePosition;
        Vector2 LastStartTile;
        Vector2 LastEndTile;

        Button LRTAManhattan;
        Button LRTAChevychev;
        Button LRTAEuclidean;
        Button FormationMode;
        Button DecisionalIA;
        Button FinalBattle;
        DistanceAlgorith CurrentLrtaAlgorithm = DistanceAlgorith.MANHATTAN;
        private List<PlayableCharacter> SelectedCharacters;
        private FormationManager ActiveFormation;
        private bool MousePressed = false;
        private bool ControlSelect = false;
        private RectangleF MouseRectangle;
        private bool IsFormationMode = false;
        private float TimeToUpdateInfluence = 0f;
        private float TimeToHeal = 0f;
        private Thread CurrentThread = null;
        private bool IsActiveIA = false;
        public const float ExecutionLRTATime = 0.3f;
        private float CurrentLRTATime = 0f;
        public const float TimeToConquer = 20f;
        private float TimeToBaseTeam1 = TimeToConquer;
        private float TimeToBaseTeam2 = TimeToConquer;
        private bool EndGame = false;

        public GameController(Kinematic mouse)
        {
            Mouse = mouse;
            LastMousePosition = mouse.Position;
            LastStartTile = LastEndTile = Vector2.Zero;
            MouseRectangle = RectangleF.Empty;
            SelectedCharacters = new List<PlayableCharacter>();
        }

        protected override void Initialize()
        {
            LRTAManhattan = EntityManager.Find<Button>("LRTA_Manhattan");
            LRTAChevychev = EntityManager.Find<Button>("LRTA_Chevychev");
            LRTAEuclidean = EntityManager.Find<Button>("LRTA_Euclidean");
            FormationMode = EntityManager.Find<Button>("FormationMode");
            DecisionalIA = EntityManager.Find<Button>("DecisionalIA");
            FinalBattle = EntityManager.Find<Button>("FinalBattle");
            
            float width = WaveServices.ViewportManager.ScreenWidth;
            float height = WaveServices.ViewportManager.ScreenHeight;
            LRTAManhattan.Entity.FindComponent<Transform2D>().Position = new Vector2(width - LRTAManhattan.Width - 10, 0);
            LRTAChevychev.Entity.FindComponent<Transform2D>().Position = new Vector2(width - LRTAChevychev.Width - 10, 50);
            LRTAEuclidean.Entity.FindComponent<Transform2D>().Position = new Vector2(width - LRTAEuclidean.Width - 10, 100);
            FormationMode.Entity.FindComponent<Transform2D>().Position = new Vector2(width - FormationMode.Width - 10, 150);
            DecisionalIA.Entity.FindComponent<Transform2D>().Position = new Vector2(width - DecisionalIA.Width - 10, 200);
            FinalBattle.Entity.FindComponent<Transform2D>().Position = new Vector2(width - FinalBattle.Width - 10, 250);

            LRTAManhattan.Click += (s, e) =>
            {
                CurrentLrtaAlgorithm = DistanceAlgorith.MANHATTAN;
                LRTAManhattan.IsVisible = false;
                LRTAChevychev.IsVisible = true;
                LRTAEuclidean.IsVisible = true;
            };

            LRTAChevychev.Click += (s, e) =>
            {
                CurrentLrtaAlgorithm = DistanceAlgorith.CHEVYCHEV;
                LRTAManhattan.IsVisible = true;
                LRTAChevychev.IsVisible = false;
                LRTAEuclidean.IsVisible = true;
            };

            LRTAEuclidean.Click += (s, e) =>
            {
                CurrentLrtaAlgorithm = DistanceAlgorith.EUCLIDEAN;
                LRTAManhattan.IsVisible = true;
                LRTAChevychev.IsVisible = true;
                LRTAEuclidean.IsVisible = false;
            };

            FormationMode.Click += (s, e) =>
            {
                IsFormationMode = !IsFormationMode;
                if (IsFormationMode)
                {
                    FormationMode.Text = "Disable Formation Mode";
                    FormationMode.BackgroundColor = Color.Red;
                }
                else
                {
                    FormationMode.Text = "Enable Formation Mode";
                    FormationMode.BackgroundColor = Color.Green;
                    ActiveFormation = null;

                    foreach (var character in EntityManager.AllEntities.Where(w => w.FindComponent<PlayableCharacter>() != null).Select(sel => sel.FindComponent<PlayableCharacter>()))
                    {
                        character.SetPathFollowing();
                    }
                }
            };

            DecisionalIA.Click += (s, e) =>
            {
                IsActiveIA = !IsActiveIA;
                if (IsActiveIA)
                {
                    DecisionalIA.Text = "Disable Decisional IA";
                    DecisionalIA.BackgroundColor = Color.Red;
                }
                else
                {
                    DecisionalIA.Text = "Enable Decisional IA";
                    DecisionalIA.BackgroundColor = Color.Green;
                }

                foreach (var character in EntityManager.AllEntitiesByComponentType(typeof(CharacterNPC)).Select(sel => sel.FindComponent<CharacterNPC>()))
                {
                    character.SetIA(IsActiveIA);
                }
            };

            FinalBattle.Click += (s, e) =>
            {
                foreach (var character in EntityManager.AllEntitiesByComponentType(typeof(CharacterNPC)).Select(sel => sel.FindComponent<CharacterNPC>()))
                {
                    character.SetIA(true);
                }
                
                var charactersEntity = EntityManager.AllEntities.Where(w => w.FindComponent<PlayableCharacter>() != null).ToList();
                while (charactersEntity.Count > 0)
                {
                    TextBlock text = EntityFactory.GetTextBlock();
                    EntityManager.Add(text);
                    var character = charactersEntity[0].FindComponent<PlayableCharacter>();
                    charactersEntity[0].RemoveComponent<PlayableCharacter>();
                    CharacterNPC npcChar;
                    charactersEntity[0].AddComponent(npcChar = character.ToCharacterNPC());
                    npcChar.Text = text;
                    npcChar.SetIA(true);
                    charactersEntity.RemoveAt(0);
                }
            };

            LRTAManhattan.IsVisible = false;
            //base.Initialize();
        }

        protected override void Update(TimeSpan gameTime)
        {
            if (EndGame) 
                return;
            float dt = (float)gameTime.TotalSeconds;
            CurrentLRTATime += dt;
            Mouse.Update(dt, new Steerings.SteeringOutput());

            SelectCharactersAndFormations(dt);

            UpdateInfluenceMap(dt);

            Heal(dt);

            Death();

            UpdateBases(dt);

            int v = Victory();

            EndGame = v != 0;

            LastMousePosition = Mouse.Position;
            DebugLines.Debug.Controller = this;
            DebugLines.Debug.Victory = v;
        }

        private void SelectCharactersAndFormations(float dt)
        {
            // Si el botón izquierdo del ratón no está pultado y la tecla estaba pulsada
            if ((WaveServices.Input.MouseState.LeftButton == WaveEngine.Common.Input.ButtonState.Release && ControlSelect))
            {
                ControlSelect = false;
            }

            // Si está suelto el botón izquierdo del raton y antes estaba pulsado
            if (WaveServices.Input.MouseState.LeftButton == WaveEngine.Common.Input.ButtonState.Release && MousePressed)
            {
                IEnumerable<PlayableCharacter> characters = EntityManager.AllEntities
                    .Where(w => w.FindComponent<PlayableCharacter>() != null)
                    .Select(s => s.FindComponent<PlayableCharacter>());

                var selectedCharacter = characters
                    .FirstOrDefault(f => Mouse.Position.IsContent(f.Kinematic.Position, new Vector2(f.Texture.Texture.Width, f.Texture.Texture.Height)));

                if (selectedCharacter != null)
                    SelectedCharacters.Add(selectedCharacter);
                MousePressed = false;
                MouseRectangle = RectangleF.Empty;
            }

            // Si está pulsado el botón izquierdo del ratón y antes no estaba pulsado
            if (WaveServices.Input.MouseState.LeftButton == WaveEngine.Common.Input.ButtonState.Pressed && !MousePressed)
            {
                // Si está pulsada la tecla control y antes no lo estaba
                if (WaveServices.Input.KeyboardState.LeftControl == WaveEngine.Common.Input.ButtonState.Pressed && !ControlSelect)
                {
                    ControlSelect = true;
                    IEnumerable<PlayableCharacter> characters = EntityManager.AllEntitiesByComponentType(typeof(PlayableCharacter))
                    .Select(s => s.FindComponent<PlayableCharacter>());

                    var selectedCharacter = characters
                        .FirstOrDefault(f => Mouse.Position.IsContent(f.Kinematic.Position, new Vector2(f.Texture.Texture.Width, f.Texture.Texture.Height)));

                    if (selectedCharacter != null)
                    {
                        if (SelectedCharacters.Contains(selectedCharacter))
                            SelectedCharacters.Remove(selectedCharacter);
                        else
                            SelectedCharacters.Add(selectedCharacter);
                    }
                }
                // Si no está, ni estaba pulsada la tecla control
                else if (!ControlSelect)
                {
                    MousePressed = true;
                    SelectedCharacters.Clear();
                    MouseRectangle.X = Mouse.Position.X;
                    MouseRectangle.Y = Mouse.Position.Y;
                }
            }

            // Si está pulsado el botón izquierdo del ratón y estaba pulsado antes
            if (WaveServices.Input.MouseState.LeftButton == WaveEngine.Common.Input.ButtonState.Pressed && MousePressed)
            {
                MouseRectangle.Width = Mouse.Position.X - MouseRectangle.X;
                MouseRectangle.Height = Mouse.Position.Y - MouseRectangle.Y;
                IEnumerable<PlayableCharacter> characters = EntityManager.AllEntitiesByComponentType(typeof(PlayableCharacter))
                    .Select(s => s.FindComponent<PlayableCharacter>());

                SelectedCharacters = characters.Where(w => w.Kinematic.Position.IsContent(MouseRectangle.Center, new Vector2(MouseRectangle.Width.Abs(), MouseRectangle.Height.Abs()))).ToList();
            }

            // Si está pulsado el botón derecho del ratón y está en una posición valida del mapa
            if (CurrentLRTATime >= ExecutionLRTATime && WaveServices.Input.MouseState.RightButton == WaveEngine.Common.Input.ButtonState.Pressed && Map.CurrentMap.PositionInMap(Mouse.Position))
            {
                if (IsFormationMode)
                {
                    if (ActiveFormation != null)
                    {
                        ActiveFormation.MoveToPosition(Mouse.Position);
                    }
                }
                else
                {
                    var enemyTarget = EntityManager.AllCharactersByTeam(2)
                        .Where(w => w is CharacterNPC).Select(s => (s as CharacterNPC))
                        .FirstOrDefault(f => Mouse.Position.IsContent(f.Kinematic.Position, new Vector2(f.Texture.Texture.Width, f.Texture.Texture.Height)));
                    if (enemyTarget != null)
                    {
                        foreach (var selectedCharacter in SelectedCharacters)
                        {
                            selectedCharacter.Attack(enemyTarget);
                        }
                    }
                    else
                    {
                        foreach (var selectedCharacter in SelectedCharacters)
                        {
                            LRTA lrta = new LRTA(selectedCharacter.Kinematic.Position, Mouse.Position, selectedCharacter.Type, CurrentLrtaAlgorithm) { UseInfluence = false };
                            LRTA lrta2 = new LRTA(selectedCharacter.Kinematic.Position, Mouse.Position, selectedCharacter.Type, CurrentLrtaAlgorithm) { UseInfluence = true };
                            if (LastStartTile != lrta.StartPos || LastEndTile != lrta.EndPos)
                            {
                                LastStartTile = lrta.StartPos;
                                LastEndTile = lrta.EndPos;
                                List<Vector2> path = lrta.Execute();
                                selectedCharacter.SetPath(path);
                            }
                            DebugLines.Debug.Path = lrta2.Execute();
                        }
                    }
                }
                CurrentLRTATime = 0f;
            }

            if (IsFormationMode)
            {
                // R para eliminar personajes
                if (WaveServices.Input.KeyboardState.F == WaveEngine.Common.Input.ButtonState.Pressed && SelectedCharacters.Count > 1)
                {
                    ActiveFormation = new FormationManager() { Pattern = new DefensiveCirclePattern() { CharacterRadius = 60f } };
                    foreach (var selectedCharacter in SelectedCharacters)
                    {
                        ActiveFormation.AddCharacter(selectedCharacter);
                    }
                    ActiveFormation.UpdateSlot();
                }

                if (ActiveFormation != null)
                    ActiveFormation.Update(dt);
            }
        }

        private void UpdateInfluenceMap(float dt)
        {
            TimeToUpdateInfluence -= dt;

            if (TimeToUpdateInfluence <= 0f)
            {
                if (CurrentThread == null || !CurrentThread.IsAlive)
                {
                    Entity[] entities = EntityManager.AllEntities.ToArray();
                    InfluenceMap.Influence.Entities = entities;
                    InfluenceMap.Influence.Texture.IsUploaded = false;
                    CurrentThread = new Thread(InfluenceMap.Influence.GenerateInfluenteMap);
                    CurrentThread.Start();
                }
                TimeToUpdateInfluence = 2f;
            }
        }

        private void Heal(float dt)
        {
            TimeToHeal -= dt;
            if (TimeToHeal <= 0f)
            {
                var characters = EntityManager.AllCharacters();

                foreach (var character in characters)
                {
                    if (Map.CurrentMap.IsInHealArea(character))
                    {
                        character.ReceiveHeal(1);
                    }
                }
                TimeToHeal = 0.3f;
            }
            
        }

        private void Death()
        {
            var characters = EntityManager.AllCharactersEntity().Where(w => (w.Components.First(f => f is ICharacterInfo) as ICharacterInfo).IsDead()).ToArray();

            foreach (var character in characters)
            {
                if (character.FindComponent<PlayableCharacter>() != null)
                {
                    SelectedCharacters.Remove(character.FindComponent<PlayableCharacter>());
                }
                (character.Components.First(f => f is ICharacterInfo) as ICharacterInfo).Dispose();
                if (character != null)
                    EntityManager.Remove(character);
            }
        }

        private int Victory()
        {
            bool anyT1 = EntityManager.AllCharacters().Any(a => a.GetTeam() == 1);
            bool anyT2 = EntityManager.AllCharacters().Any(a => a.GetTeam() == 2);
            int res1 = anyT1 ? (anyT2 ? 0 : 1) : 2;
            int res2 = TimeToBaseTeam1 <= 0 ? 2 : (TimeToBaseTeam2 <= 0 ? 1 : 0);
            return (res1 != 0 ? res1 : res2);
        }

        private void UpdateBases(float dt)
        {
            int b1 = EntityManager.AllCharactersByTeam(1).Count(w => Map.CurrentMap.IsInBase(w.GetPosition(), 2));
            int b2 = EntityManager.AllCharactersByTeam(2).Count(w => Map.CurrentMap.IsInBase(w.GetPosition(), 1));

            if (b1 == 0)
                TimeToBaseTeam2 = TimeToConquer;
            if (b2 == 0)
                TimeToBaseTeam1 = TimeToConquer;
            TimeToBaseTeam1 -= b2 * dt;
            TimeToBaseTeam2 -= b1 * dt;
        }

        public void Draw(LineBatch2D lb)
        {
            lb.DrawRectangleVM(MouseRectangle, Color.Green, 1f);

            foreach (var selectedCharacter in SelectedCharacters)
            {
                selectedCharacter.Draw(lb);
            }
            if (ActiveFormation != null)
                ActiveFormation.Draw(lb);
        }

    }

   
}
