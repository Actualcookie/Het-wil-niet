using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace Pong 
{ 
   
    public class gameWorld : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D background,life, paddle, ball,GameOver, player2wins, player1wins;
        Vector2 positionlives, angle;
        Paddles paddle1, paddle2;
        int gamestate, player1lives, player2lives;
        KeyboardState keyboardState;
        float delta, acceleration, hitLocation,reflectionAngle, normalizedLocation, velocity ;
        Ball Ball;
        Color color;


    static void Main()
    {
     gameWorld game = new gameWorld();
     game.Run();
    }
    public gameWorld()
    {
      Content.RootDirectory="Content";
      graphics= new GraphicsDeviceManager(this);
      graphics.PreferredBackBufferHeight = 600;
      graphics.PreferredBackBufferWidth = 1000;
   }
      
    protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            /*      life = Content.Load<Texture2D> ("spr_Life");
                  background = Content.Load<Texture2D> ("spr_Background");
                 Gamover = Content.Load<Texture2D>("spr_GameOver");
             *    player2wins= Content.Load<Texture2D> ("spr_P2wins")
             *    player1wins= Content.Load<Texture2D> ("spr_P1wins")
             */
            paddle = Content.Load<Texture2D> ("spr_paddle");
            ball = Content.Load<Texture2D> ("spr_ball");
            paddle1 = new Paddles();
            velocity = 200;
            paddle1.position = new Vector2(0, GraphicsDevice.Viewport.Height / 2 - paddle.Height / 2);
            paddle1.velocity = velocity;
            paddle2 = new Paddles();
            paddle2.position = new Vector2(GraphicsDevice.Viewport.Width - paddle.Width, GraphicsDevice.Viewport.Height / 2 - paddle.Height / 2);
            paddle2.velocity = velocity;
            Ball = new Ball();
            Ball.position = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2 - ball.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2 - ball.Height / 2);
            Ball.velocity = velocity;
            gamestate = 0;
            acceleration = 1.07f;
            color = Color.White;
            angle = new Vector2(1,0);
            player1lives = 3;
            player2lives = 3;
            
        }
    protected override void Update(GameTime gameTime)
    {
        SetRectangles();
        delta =(float)gameTime.ElapsedGameTime.TotalSeconds;
        keyboardState = Keyboard.GetState();
        
        if (gamestate == 0) 
        {
        IntroScreen();
        }
        else if (gamestate == 1)
        {
        HandleInput();
        BallMovement();
        }
      else if (gamestate == 2)
        {
            GameOverScreen();
        }
        
    }
    
      protected override void Draw(GameTime gameTime)
    {
 	spriteBatch.Begin();
        spriteBatch.Draw(paddle, paddle1.position, Color.Red);
        spriteBatch.Draw(paddle, paddle2.position, Color.Blue);
        spriteBatch.Draw(ball, Ball.position, color);
        spriteBatch.End();
    }
     //extra methods
    public void HandleInput()
    { //player 1
        if (keyboardState.IsKeyDown(Keys.S))
        { 
            if(paddle1.position.Y + paddle.Height < GraphicsDevice.Viewport.Bounds.Height){
                paddle1.position.Y += paddle1.velocity * delta;
            }   
        }
        else if (keyboardState.IsKeyDown(Keys.W))
        {
            if(paddle1.position.Y > 0){
                paddle1.position.Y -= paddle1.velocity * delta;
            }
            
        }  //player 2
        if(keyboardState.IsKeyDown(Keys.Down)){
            if(paddle2.position.Y + paddle.Height < GraphicsDevice.Viewport.Bounds.Height){
                paddle2.position.Y += paddle2.velocity * delta;
            }
        }
        else if(keyboardState.IsKeyDown(Keys.Up))
        {
            if(paddle2.position.Y > 0){
                paddle2.position.Y -= paddle2.velocity * delta;
            }
        }
    }

    public void BallMovement() 
    {
        if(Ball.position.X <= 0)
        {
            Ball.position = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2 - ball.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2 - ball.Height / 2);// punt player2
            player1lives -= 1;
            RandomizeDirection();
        }
        else if (Ball.position.X + ball.Width >= GraphicsDevice.Viewport.Bounds.Width)
        {
           Ball.position = new Vector2(GraphicsDevice.Viewport.Bounds.Width / 2 - ball.Width / 2, GraphicsDevice.Viewport.Bounds.Height / 2 - ball.Height / 2);
           player2lives -= 1;
            RandomizeDirection();
            //punt player1
        }
        else 
        {
            if (Ball.position.Y <= 0 )
            {
                angle.Y *= -1;
                Ball.position.Y = 1;
            }
            if(Ball.position.Y+ball.Height >= GraphicsDevice.Viewport.Bounds.Height)
            {
                angle.Y *= -1;
                Ball.position.Y = GraphicsDevice.Viewport.Bounds.Height - ball.Height -1;
            }
                
            if(paddle1.rectangle.Intersects(Ball.rectangle))
            {
                Reflection1();
                color = Color.Red;
                Ball.position.X = paddle.Width + 1;
               Accelerate();
            }
            if (paddle2.rectangle.Intersects(Ball.rectangle))
            {
                Reflection2();
                color = Color.Blue;
                Ball.position.X =GraphicsDevice.Viewport.Bounds.Width - (paddle.Width+1+ball.Width);
                Accelerate();

            }

            
        }
        Ball.position += (Ball.velocity *angle * delta);
      
    }

    public void SetRectangles(){
    Ball.rectangle = new Rectangle((int)Ball.position.X,(int)Ball.position.Y, ball.Width, ball.Height);
    paddle1.rectangle = new Rectangle((int)paddle1.position.X,(int)paddle1.position.Y, paddle.Width, paddle.Height);
    paddle2.rectangle = new Rectangle((int)paddle2.position.X,(int)paddle2.position.Y, paddle.Width, paddle.Height);

    }

    public void Reflection1()
    {
        hitLocation = (paddle1.position.Y + paddle.Height / 2) - Ball.position.Y;
        normalizedLocation = hitLocation / (paddle.Height / 2);
        reflectionAngle = normalizedLocation * ((55*(float)Math.PI)/180);
        angle.X = (float)Math.Cos(reflectionAngle);
        angle.Y = -(float)Math.Sin(reflectionAngle);
    }

    public void Reflection2()
    {
        hitLocation = (paddle2.position.Y + paddle.Height / 2) - Ball.position.Y;
        normalizedLocation = hitLocation / (paddle.Height / 2);
        reflectionAngle = normalizedLocation * ((55 * (float)Math.PI) / 180);
        angle.X = (float)Math.Cos(reflectionAngle);
        angle.Y = -(float)Math.Sin(reflectionAngle);
        angle.X = -angle.X;
    }

    public void Accelerate()
    {
        velocity *= acceleration;
        Ball.velocity = velocity;
        paddle1.velocity =velocity;
        paddle2.velocity = velocity;

    }

    public void IntroScreen()
    {
        if(keyboardState.IsKeyDown(Keys.Space))
        {
            gamestate = 1;
            RandomizeDirection();

        }
    }

    public void GameOverScreen()
    {

  spriteBatch.Begin();
       
        if (player1lives == 0)
        { 
        spritebatch.Draw(GameOver, new Vector2((GraphicsDevice.Viewport.Bounds.Width - GameOver.Width), 0), Color.Aquamarine);
   spriteBatch.Draw(player2wins, new Vector2(GraphicsDevice.Viewport.Bounds.Width-player2wins.Width, GraphicsDevice.Viewport.Bounds.Height/2), Color.Aquamarine); 
           }
        else if (player2lives == 0)
        { 
       spritebatch.Draw(GameOver, new Vector2((GraphicsDevice.Viewport.Bounds.Width/2 - GameOver.Width), 0), Color.Red);
              spriteBatch.Draw(player1wins, new Vector2(GraphicsDevice.Viewport.Bounds.Width-player1wins.Width, GraphicsDevice.Viewport.Bounds.Height/2), Color.Red);
   }
      spriteBatch.End();
    }
    }

    public void RandomizeDirection()
    {
        Random rnd = new Random();
        float x = rnd.Next(-150, 150);
        angle.Y = x / 100;
        angle.X = 1;
        if (rnd.Next(0, 100) < 50)
        {
            angle.X *= -1;
        }
        velocity = 200;
        Ball.velocity = velocity;
        paddle1.velocity = velocity;
        paddle2.velocity = velocity;

>>>>>>> origin/master
    }
    // end extra methods
    
    }
   

    class Paddles
    {
      public Vector2 position;
      public float velocity;
      public  Color color;
      public Rectangle rectangle;
    }

    class Ball
    {
        public Vector2 position ;
        public Color color;
        public Rectangle rectangle;
        public float velocity;
    }
}