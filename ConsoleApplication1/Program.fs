open System
open System.Drawing
open System.Windows.Forms
Console.Title <- "Различные примеры (F#) :: Рисование мышкой"
open System.Drawing.Drawing2D
let drawingForm = new Form(Text = "Различные примеры (F#) :: Рисование мышкой", AutoScaleDimensions = new System.Drawing.SizeF(60.0F, 13.0F), ClientSize = new System.Drawing.Size(500, 250), StartPosition = FormStartPosition.CenterScreen) //, FormBorderStyle = FormBorderStyle.None

let exitButton3 = new Button(Text = "Выход", Location = new System.Drawing.Point(400, 210))
let eraseButton3 = new Button(Text = "Очистить", Location = new System.Drawing.Point(320, 210))
let colorButton3 = new Button(Text = "Цвет пера", Location = new System.Drawing.Point(240, 210))

drawingForm.Controls.Add(exitButton3)
drawingForm.Controls.Add(eraseButton3)
drawingForm.Controls.Add(colorButton3)

exitButton3.Anchor <- AnchorStyles.Right
exitButton3.Anchor <- AnchorStyles.Bottom

let menu = new System.Windows.Forms.MenuStrip(Location = new System.Drawing.Point(0, 0), Name = "menu")
let close_TSMI = new System.Windows.Forms.ToolStripMenuItem(Name = "close_TSMI", Text = "Close", Size = new System.Drawing.Size(50, 20))
let edit_rectangle_TSMI = new System.Windows.Forms.ToolStripMenuItem(Name = "edit_rectangle_TSMI", Text = "New Area", Size = new System.Drawing.Size(50, 20))

close_TSMI.Click.Add(fun quit3 -> drawingForm.Close())

ignore(menu.Items.Add(edit_rectangle_TSMI))
ignore(menu.Items.Add(close_TSMI))
drawingForm.Controls.Add(menu)

let createGraphics = drawingForm.CreateGraphics()
createGraphics.SmoothingMode<-SmoothingMode.HighQuality



type mol = class
    val mutable x: float32
    val mutable y: float32
    val mutable vy: float32
    val mutable vx: float32
    val r: float32
    val mas: float32
    val color : Color
    new (_x, _y, _vx, _vy, _r, _mas, _color) =
        { x = _x; y = _y; vx = _vx; vy = _vy; r = _r; mas = _mas; color = _color}
    member m.Move (t : float32) =
        m.x <- m.vx*t + m.x
        m.y <- m.vy*t + m.y
end

type border = class
    val mutable x0: float32
    val mutable y0: float32
    val mutable x1: float32
    val mutable y1: float32
    
    val mutable vx0: float32
    val mutable vy0: float32
    val mutable vx1: float32
    val mutable vy1: float32

    val color : Color
    new (_x0, _y0, _x1, _y1, _vx0, _vy0, _vx1, _vy1, _color) =
        {x0 = _x0; y0 = _y0; x1 = _x1; y1 = _y1; vx0 = _vx0; vy0 = _vy0; vx1 = _vx1; vy1 = _vy1; color = _color}
    member m.Move (t : float32) =
        m.x0 <- m.vx0*t + m.x0
        m.y0 <- m.vy0*t + m.y0
        m.x1 <- m.vx1*t + m.x1
        m.y1 <- m.vy1*t + m.y1
end

let board_array = [|
    new border(50.0f, 50.0f, 500.0f, 50.0f, 0.0f, 0.0f, 0.0f, 0.0f, Color.Black) 
    new border(500.0f, 50.0f, 500.0f, 500.0f, 0.0f, 0.0f, 0.0f, 0.0f, Color.Black) 
    new border(500.0f, 500.0f, 50.0f, 500.0f, 0.0f, 0.0f, 0.0f, 0.0f, Color.Black) 
    new border(50.0f, 50.0f, 50.0f, 500.0f, 0.0f, 0.0f, 0.0f, 0.0f, Color.Black) 
    |]

let mol_array = [|
    new mol(150.0f, 150.0f, 5.0f, 5.0f, 10.0f, 10.0f, Color.Red)
    new mol(200.0f, 200.0f, 0.0f, 0.0f, 20.0f, 40.0f, Color.Orange)
    |]

let colorDialog3 = new ColorDialog()
let mutable color3 = new ColorBlend()

drawingForm.Load.Add(fun background3 -> color3.Colors <- [|Color.Indigo|])

let mouse_click_handler = //событие клика мыши (делегат)
    new MouseEventHandler (
        fun sender args->
            if (args.Button = System.Windows.Forms.MouseButtons.Left) then 
                createGraphics.FillRectangle(new SolidBrush(color3.Colors.[0]),new Rectangle(args.X,args.Y,5,5)))

drawingForm.MouseMove.AddHandler(mouse_click_handler)
eraseButton3.Click.Add(fun erase3-> 
    createGraphics.Clear(drawingForm.BackColor) 
    drawingForm.MouseMove.RemoveHandler(mouse_click_handler)
    drawingForm.Controls.Remove(eraseButton3))

    
exitButton3.Click.Add(fun quit3 -> drawingForm.Close())
colorButton3.Click.Add(fun colors3 ->
    if colorDialog3.ShowDialog() = DialogResult.OK then 
        color3.Colors<-[|colorDialog3.Color|])

let tick_delay = 200
let timer = new System.Windows.Forms.Timer(Interval = tick_delay)

let mutable ready_to_paint = true

let boar_mas i = Array.get board_array i
let mol_mas i = Array.get mol_array i

let paint_all = 
    for i in 0 .. board_array.Length - 1 do
        createGraphics.DrawLine(new Pen(boar_mas(i).color, 5.0f), boar_mas(i).vx0, boar_mas(i).vy0, boar_mas(i).vx1, boar_mas(i).vy1)
    for i in 0 .. mol_array.Length - 1 do
        createGraphics.FillEllipse(new SolidBrush(mol_mas(i).color), mol_mas(i).x, mol_mas(i).y, mol_mas(i).r, mol_mas(i).r)

let move_all = 
    for i in 0 .. board_array.Length - 1 do
        boar_mas(i).Move((float32)tick_delay/1000.0f)
    for i in 0 .. mol_array.Length - 1 do
        mol_mas(i).Move((float32)tick_delay/1000.0f)

let timer_tick = 
    new EventHandler(
        fun sender args->
            if ready_to_paint = true then 
                ignore(ready_to_paint = false)
                move_all
                createGraphics.Clear(SystemColors.Control)
                paint_all
                ignore(ready_to_paint = true))

timer.Tick.AddHandler(timer_tick)

timer.Start()
Application.Run(drawingForm)
printfn "\t\t\tНажмите клавишу Enter для продолжения..."
//Console.ReadKey()
Console.Clear()