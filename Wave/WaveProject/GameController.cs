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
        // Posición del ratón
        Kinematic Mouse;
        // Posición anterior del ratón
        Vector2 LastMousePosition;
        // Último tile de inicio de un pathfinding
        Vector2 LastStartTile;
        // Último tile de fin de un pathfinding
        Vector2 LastEndTile;

        // Botones
        Button LRTAManhattan;
        Button LRTAChevychev;
        Button LRTAEuclidean;
        Button FormationMode;
        Button DecisionalIA;
        Button FinalBattle;
        Button SetDebug;

        // Algoritmo (heurístico) que está usando actualmente el pathfinding
        DistanceAlgorith CurrentLrtaAlgorithm = DistanceAlgorith.MANHATTAN;

        // Jugadores seleccionados
        private List<PlayableCharacter> SelectedCharacters;
        // Formación actual
        private FormationManager ActiveFormation;
        // Si el click izquierdo está pulsado
        private bool MousePressed = false;
        // Si el botón control está pulsado
        private bool ControlSelect = false;
        // Rectángulo de selección de jugadores del mouse
        private RectangleF MouseRectangle;
        // Indica si estamos en modo formación
        private bool IsFormationMode = false;
        // Contador de tiempo para actualizar el mapa influencia
        private float TimeToUpdateInfluence = 0f;
        // Contador de tiempo para curar personajes en área de curación
        private float TimeToHeal = 0f;
        // Hilo en ejecución actual para el mapa de influencia
        private Thread CurrentThread = null;
        // Indica si la IA está activa
        private bool IsActiveIA = false;
        // Tiempo para ejecutar de nuevo un LRTA*
        public const float ExecutionLRTATime = 0.3f;
        // Contador de tiempo restante para ejecutar el LRTA*
        private float CurrentLRTATime = 0f;
        // Tiempo necesario para conquistar una base y ganar
        public const float TimeToConquer = 20f;
        // Tiempo restante para conquistar la base del equipo 1
        private float TimeToBaseTeam1 = TimeToConquer;
        // Tiempo restante para conquistar la base del equipo 2
        private float TimeToBaseTeam2 = TimeToConquer;
        // Indica si el jeugo ha acabado
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
            // Buscamos los botones
            LRTAManhattan = EntityManager.Find<Button>("LRTA_Manhattan");
            LRTAChevychev = EntityManager.Find<Button>("LRTA_Chevychev");
            LRTAEuclidean = EntityManager.Find<Button>("LRTA_Euclidean");
            FormationMode = EntityManager.Find<Button>("FormationMode");
            DecisionalIA = EntityManager.Find<Button>("DecisionalIA");
            FinalBattle = EntityManager.Find<Button>("FinalBattle");
            SetDebug = EntityManager.Find<Button>("SetDebug");
            
            // Establecemos su posición en la pantalla
            float width = WaveServices.ViewportManager.ScreenWidth;
            float height = WaveServices.ViewportManager.ScreenHeight;
            LRTAManhattan.Entity.FindComponent<Transform2D>().Position = new Vector2(width - LRTAManhattan.Width - 10, 0);
            LRTAChevychev.Entity.FindComponent<Transform2D>().Position = new Vector2(width - LRTAChevychev.Width - 10, 50);
            LRTAEuclidean.Entity.FindComponent<Transform2D>().Position = new Vector2(width - LRTAEuclidean.Width - 10, 100);
            FormationMode.Entity.FindComponent<Transform2D>().Position = new Vector2(width - FormationMode.Width - 10, 150);
            DecisionalIA.Entity.FindComponent<Transform2D>().Position = new Vector2(width - DecisionalIA.Width - 10, 200);
            FinalBattle.Entity.FindComponent<Transform2D>().Position = new Vector2(width - FinalBattle.Width - 10, 250);
            SetDebug.Entity.FindComponent<Transform2D>().Position = new Vector2(width - FinalBattle.Width - 10, 300);

            // Establecemos el comportamiento de los botones
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

            SetDebug.Click += (s, e) =>
            {
                DebugLines.Debug.IsDebugging = !DebugLines.Debug.IsDebugging;
                if (DebugLines.Debug.IsDebugging)
                {
                    SetDebug.Text = "Disable Debug";
                    SetDebug.BackgroundColor = Color.Red;
                }
                else
                {
                    SetDebug.Text = "Enable Debug";
                    SetDebug.BackgroundColor = Color.Green;
                }
            };

            LRTAManhattan.IsVisible = false;

            DebugLines.Debug.Controller = this;
            //base.Initialize();
        }

        protected override void Update(TimeSpan gameTime)
        {
            // Si ha acabado el juego salimos del bucle
            if (EndGame) 
                return;

            float dt = (float)gameTime.TotalSeconds; // Establecemos el delta time
            CurrentLRTATime += dt; // Aumentamos el tiempo que hace desde el último LRTA*
            Mouse.Update(dt, new Steerings.SteeringOutput()); // Actualizamos la posición del ratón

            SelectCharactersAndFormations(dt); // Ejecutamos el proceso de selección de personajes

            UpdateInfluenceMap(dt); // Comprobamos si hay que actualizar el mapa de influecia

            Heal(dt); // Comprobamos si hay que curar al algún personaje

            Death(); // Comprobamos si han muerto personajes y los eliminamos

            UpdateBases(dt); // Actualizamos los tiempos de conquista de bases

            int v = Victory(); // Comprobamos si algún bando ha ganado

            EndGame = v != 0; // Se establece el fin del juego en base ha si ha ganado algún bando

            LastMousePosition = Mouse.Position; // Guardamos la última posición del ratón
            DebugLines.Debug.Victory = v; // Establecemos en el Debug (quién pinta información en pantalla) el ganador
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
                // Obtiene todos los jugadores controlables
                IEnumerable<PlayableCharacter> characters = EntityManager.AllEntities
                    .Where(w => w.FindComponent<PlayableCharacter>() != null)
                    .Select(s => s.FindComponent<PlayableCharacter>());

                // Se queda con el que está marcado por el ratón
                var selectedCharacter = characters
                    .FirstOrDefault(f => Mouse.Position.IsContent(f.Kinematic.Position, new Vector2(f.Texture.Texture.Width, f.Texture.Texture.Height)));

                // Si había alguno lo añade a la lista
                if (selectedCharacter != null)
                    SelectedCharacters.Add(selectedCharacter);
                MousePressed = false; // Establecemos el click izquierdo como liberado
                MouseRectangle = RectangleF.Empty; // Eliminamos el rectángulo del ratón
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

            // Si está pulsado el botón derecho del ratón y está en una posición valida del mapa y toca ejecutar LRTA*
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

            if (TimeToUpdateInfluence <= 0f) // Si toca actualizar el mapa de influencia
            {
                if (CurrentThread == null || !CurrentThread.IsAlive) // Comprobamos que el último hilo haya acabado
                {
                    Entity[] entities = EntityManager.AllEntities.ToArray();
                    InfluenceMap.Influence.Entities = entities;
                    InfluenceMap.Influence.Texture.IsUploaded = false;
                    CurrentThread = new Thread(InfluenceMap.Influence.GenerateInfluenteMap);
                    CurrentThread.Start();
                }
                TimeToUpdateInfluence = 2f; // Reiniciamos el tiempo para refresco
            }
        }

        private void Heal(float dt)
        {
            TimeToHeal -= dt;
            if (TimeToHeal <= 0f) // Si toca curar personajes
            {
                var characters = EntityManager.AllCharacters();

                foreach (var character in characters)
                {
                    // Curamos los que están en el área de curación
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
            // Cogemos todos los personajes que estén muertos
            var characters = EntityManager.AllCharactersEntity().Where(w => (w.Components.First(f => f is ICharacterInfo) as ICharacterInfo).IsDead()).ToArray();

            // Los eliminamos por completo
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
            // Comprobamos si queda algun personaje del equipo 1
            bool anyT1 = EntityManager.AllCharacters().Any(a => a.GetTeam() == 1);
            // Comprobamos si queda algun personaje del equipo 2
            bool anyT2 = EntityManager.AllCharacters().Any(a => a.GetTeam() == 2);
            // Devuelve 1 si no quedan jugadores del equipo 2, y 2 si no quedan jugadores del equipo 1
            int res1 = anyT1 ? (anyT2 ? 0 : 1) : 2;
            // Devuelve 1 si se ha conquistado la base 2, y 1 si se ha conquistado la base 1
            int res2 = TimeToBaseTeam1 <= 0 ? 2 : (TimeToBaseTeam2 <= 0 ? 1 : 0);
            // Finalmente, devuelve el número del equipo ganador, o 0 si aún no ha ganado nadie
            return (res1 != 0 ? res1 : res2);
        }

        private void UpdateBases(float dt)
        {
            // Obtiene cuantos personajes del equipo 1 están en la base del equipo 2
            int b1 = EntityManager.AllCharactersByTeam(1).Count(w => Map.CurrentMap.IsInBase(w.GetPosition(), 2));
            // Obtiene cuantos personajes del equipo 2 están en la base del equipo 1
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
