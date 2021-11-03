
namespace SoundBankEditor
{
    partial class ProgramBankEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.listViewPrograms = new System.Windows.Forms.ListView();
            this.columnProgramID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnProgramName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnProgramSplitsNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnProgramSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSizeAsHexadecimalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listViewLayers = new System.Windows.Forms.ListView();
            this.columnLayerID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLayerBendMinus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLayerBendPlus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLayerDelay = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLayerSplitNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnLayerSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label3 = new System.Windows.Forms.Label();
            this.listViewSplits = new System.Windows.Forms.ListView();
            this.columnSplitID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSplitNoteStart = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSplitNoteEnd = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSplitDirect = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSplitPanpot = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSplitFXLevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSplitFXChannel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnSplitSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.buttonMoveProgramUp = new System.Windows.Forms.Button();
            this.buttonMoveProgramDown = new System.Windows.Forms.Button();
            this.buttonMoveLayerDown = new System.Windows.Forms.Button();
            this.buttonMoveLayerUp = new System.Windows.Forms.Button();
            this.buttonMoveSplitDown = new System.Windows.Forms.Button();
            this.buttonMoveSplitUp = new System.Windows.Forms.Button();
            this.buttonAddProgram = new System.Windows.Forms.Button();
            this.buttonDeleteProgram = new System.Windows.Forms.Button();
            this.buttonDeleteSplit = new System.Windows.Forms.Button();
            this.buttonAddSplit = new System.Windows.Forms.Button();
            this.buttonDeleteLayer = new System.Windows.Forms.Button();
            this.buttonAddLayer = new System.Windows.Forms.Button();
            this.buttonProgramProperties = new System.Windows.Forms.Button();
            this.buttonLayerProperties = new System.Windows.Forms.Button();
            this.buttonSplitProperties = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusTextFilename = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusTextVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusTextNumPrograms = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonSplitPlay = new System.Windows.Forms.Button();
            this.contextMenuSplit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemExportWaveform = new System.Windows.Forms.ToolStripMenuItem();
            this.exportRawWaveformToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonSplitStop = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.contextMenuSplit.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewPrograms
            // 
            this.listViewPrograms.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnProgramID,
            this.columnProgramName,
            this.columnProgramSplitsNo,
            this.columnProgramSize});
            this.listViewPrograms.FullRowSelect = true;
            this.listViewPrograms.GridLines = true;
            this.listViewPrograms.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewPrograms.HideSelection = false;
            this.listViewPrograms.Location = new System.Drawing.Point(12, 60);
            this.listViewPrograms.MultiSelect = false;
            this.listViewPrograms.Name = "listViewPrograms";
            this.listViewPrograms.Size = new System.Drawing.Size(366, 701);
            this.listViewPrograms.TabIndex = 0;
            this.listViewPrograms.UseCompatibleStateImageBehavior = false;
            this.listViewPrograms.View = System.Windows.Forms.View.Details;
            this.listViewPrograms.SelectedIndexChanged += new System.EventHandler(this.listViewPrograms_SelectedIndexChanged);
            this.listViewPrograms.DoubleClick += new System.EventHandler(this.listViewPrograms_DoubleClick);
            // 
            // columnProgramID
            // 
            this.columnProgramID.Text = "ID";
            // 
            // columnProgramName
            // 
            this.columnProgramName.Text = "Name";
            this.columnProgramName.Width = 103;
            // 
            // columnProgramSplitsNo
            // 
            this.columnProgramSplitsNo.Text = "Splits";
            this.columnProgramSplitsNo.Width = 76;
            // 
            // columnProgramSize
            // 
            this.columnProgramSize.Text = "Size";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1176, 33);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Image = global::SoundBankEditor.Properties.Resources._new;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(170, 34);
            this.newToolStripMenuItem.Text = "&New";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::SoundBankEditor.Properties.Resources.open;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(170, 34);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(170, 34);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showSizeAsHexadecimalToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(65, 29);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // showSizeAsHexadecimalToolStripMenuItem
            // 
            this.showSizeAsHexadecimalToolStripMenuItem.CheckOnClick = true;
            this.showSizeAsHexadecimalToolStripMenuItem.Name = "showSizeAsHexadecimalToolStripMenuItem";
            this.showSizeAsHexadecimalToolStripMenuItem.Size = new System.Drawing.Size(321, 34);
            this.showSizeAsHexadecimalToolStripMenuItem.Text = "Show Size as Hexadecimal";
            this.showSizeAsHexadecimalToolStripMenuItem.Click += new System.EventHandler(this.showSizeAsHexadecimalToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "MIDI Programs:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(418, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Program Layers:";
            // 
            // listViewLayers
            // 
            this.listViewLayers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnLayerID,
            this.columnLayerBendMinus,
            this.columnLayerBendPlus,
            this.columnLayerDelay,
            this.columnLayerSplitNo,
            this.columnLayerSize});
            this.listViewLayers.FullRowSelect = true;
            this.listViewLayers.GridLines = true;
            this.listViewLayers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewLayers.HideSelection = false;
            this.listViewLayers.Location = new System.Drawing.Point(422, 60);
            this.listViewLayers.MultiSelect = false;
            this.listViewLayers.Name = "listViewLayers";
            this.listViewLayers.Size = new System.Drawing.Size(709, 184);
            this.listViewLayers.TabIndex = 4;
            this.listViewLayers.UseCompatibleStateImageBehavior = false;
            this.listViewLayers.View = System.Windows.Forms.View.Details;
            this.listViewLayers.SelectedIndexChanged += new System.EventHandler(this.listViewLayers_SelectedIndexChanged);
            this.listViewLayers.DoubleClick += new System.EventHandler(this.listViewLayers_DoubleClick);
            // 
            // columnLayerID
            // 
            this.columnLayerID.Text = "ID";
            this.columnLayerID.Width = 45;
            // 
            // columnLayerBendMinus
            // 
            this.columnLayerBendMinus.Text = "Bend Range -";
            this.columnLayerBendMinus.Width = 118;
            // 
            // columnLayerBendPlus
            // 
            this.columnLayerBendPlus.Text = "Bend Range +";
            this.columnLayerBendPlus.Width = 122;
            // 
            // columnLayerDelay
            // 
            this.columnLayerDelay.Text = "Delay";
            this.columnLayerDelay.Width = 62;
            // 
            // columnLayerSplitNo
            // 
            this.columnLayerSplitNo.Text = "Splits";
            this.columnLayerSplitNo.Width = 80;
            // 
            // columnLayerSize
            // 
            this.columnLayerSize.Text = "Size";
            this.columnLayerSize.Width = 137;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(418, 256);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 20);
            this.label3.TabIndex = 5;
            this.label3.Text = "Layer Splits:";
            // 
            // listViewSplits
            // 
            this.listViewSplits.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnSplitID,
            this.columnSplitNoteStart,
            this.columnSplitNoteEnd,
            this.columnSplitDirect,
            this.columnSplitPanpot,
            this.columnSplitFXLevel,
            this.columnSplitFXChannel,
            this.columnSplitSize});
            this.listViewSplits.FullRowSelect = true;
            this.listViewSplits.GridLines = true;
            this.listViewSplits.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewSplits.HideSelection = false;
            this.listViewSplits.Location = new System.Drawing.Point(422, 279);
            this.listViewSplits.MultiSelect = false;
            this.listViewSplits.Name = "listViewSplits";
            this.listViewSplits.Size = new System.Drawing.Size(709, 482);
            this.listViewSplits.TabIndex = 6;
            this.listViewSplits.UseCompatibleStateImageBehavior = false;
            this.listViewSplits.View = System.Windows.Forms.View.Details;
            this.listViewSplits.SelectedIndexChanged += new System.EventHandler(this.listViewSplits_SelectedIndexChanged);
            this.listViewSplits.DoubleClick += new System.EventHandler(this.listViewSplits_DoubleClick);
            this.listViewSplits.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewSplits_MouseClick);
            // 
            // columnSplitID
            // 
            this.columnSplitID.Text = "ID";
            this.columnSplitID.Width = 45;
            // 
            // columnSplitNoteStart
            // 
            this.columnSplitNoteStart.Text = "Start Note";
            this.columnSplitNoteStart.Width = 97;
            // 
            // columnSplitNoteEnd
            // 
            this.columnSplitNoteEnd.Text = "End Note";
            this.columnSplitNoteEnd.Width = 97;
            // 
            // columnSplitDirect
            // 
            this.columnSplitDirect.Text = "Direct Lev";
            this.columnSplitDirect.Width = 93;
            // 
            // columnSplitPanpot
            // 
            this.columnSplitPanpot.Text = "Panpot";
            this.columnSplitPanpot.Width = 77;
            // 
            // columnSplitFXLevel
            // 
            this.columnSplitFXLevel.Text = "FX Lev";
            this.columnSplitFXLevel.Width = 66;
            // 
            // columnSplitFXChannel
            // 
            this.columnSplitFXChannel.Text = "FX Ch";
            this.columnSplitFXChannel.Width = 72;
            // 
            // columnSplitSize
            // 
            this.columnSplitSize.Text = "Size";
            this.columnSplitSize.Width = 91;
            // 
            // buttonMoveProgramUp
            // 
            this.buttonMoveProgramUp.Location = new System.Drawing.Point(384, 60);
            this.buttonMoveProgramUp.Name = "buttonMoveProgramUp";
            this.buttonMoveProgramUp.Size = new System.Drawing.Size(32, 32);
            this.buttonMoveProgramUp.TabIndex = 7;
            this.buttonMoveProgramUp.Text = "↑";
            this.buttonMoveProgramUp.UseVisualStyleBackColor = true;
            // 
            // buttonMoveProgramDown
            // 
            this.buttonMoveProgramDown.Location = new System.Drawing.Point(384, 98);
            this.buttonMoveProgramDown.Name = "buttonMoveProgramDown";
            this.buttonMoveProgramDown.Size = new System.Drawing.Size(32, 32);
            this.buttonMoveProgramDown.TabIndex = 8;
            this.buttonMoveProgramDown.Text = "↓";
            this.buttonMoveProgramDown.UseVisualStyleBackColor = true;
            // 
            // buttonMoveLayerDown
            // 
            this.buttonMoveLayerDown.Location = new System.Drawing.Point(1137, 98);
            this.buttonMoveLayerDown.Name = "buttonMoveLayerDown";
            this.buttonMoveLayerDown.Size = new System.Drawing.Size(32, 32);
            this.buttonMoveLayerDown.TabIndex = 12;
            this.buttonMoveLayerDown.Text = "↓";
            this.buttonMoveLayerDown.UseVisualStyleBackColor = true;
            // 
            // buttonMoveLayerUp
            // 
            this.buttonMoveLayerUp.Location = new System.Drawing.Point(1137, 60);
            this.buttonMoveLayerUp.Name = "buttonMoveLayerUp";
            this.buttonMoveLayerUp.Size = new System.Drawing.Size(32, 32);
            this.buttonMoveLayerUp.TabIndex = 11;
            this.buttonMoveLayerUp.Text = "↑";
            this.buttonMoveLayerUp.UseVisualStyleBackColor = true;
            // 
            // buttonMoveSplitDown
            // 
            this.buttonMoveSplitDown.Location = new System.Drawing.Point(1137, 316);
            this.buttonMoveSplitDown.Name = "buttonMoveSplitDown";
            this.buttonMoveSplitDown.Size = new System.Drawing.Size(32, 32);
            this.buttonMoveSplitDown.TabIndex = 14;
            this.buttonMoveSplitDown.Text = "↓";
            this.buttonMoveSplitDown.UseVisualStyleBackColor = true;
            // 
            // buttonMoveSplitUp
            // 
            this.buttonMoveSplitUp.Location = new System.Drawing.Point(1137, 278);
            this.buttonMoveSplitUp.Name = "buttonMoveSplitUp";
            this.buttonMoveSplitUp.Size = new System.Drawing.Size(32, 32);
            this.buttonMoveSplitUp.TabIndex = 13;
            this.buttonMoveSplitUp.Text = "↑";
            this.buttonMoveSplitUp.UseVisualStyleBackColor = true;
            // 
            // buttonAddProgram
            // 
            this.buttonAddProgram.Location = new System.Drawing.Point(383, 136);
            this.buttonAddProgram.Name = "buttonAddProgram";
            this.buttonAddProgram.Size = new System.Drawing.Size(32, 32);
            this.buttonAddProgram.TabIndex = 15;
            this.buttonAddProgram.Text = "+";
            this.buttonAddProgram.UseVisualStyleBackColor = true;
            // 
            // buttonDeleteProgram
            // 
            this.buttonDeleteProgram.Location = new System.Drawing.Point(383, 174);
            this.buttonDeleteProgram.Name = "buttonDeleteProgram";
            this.buttonDeleteProgram.Size = new System.Drawing.Size(32, 32);
            this.buttonDeleteProgram.TabIndex = 16;
            this.buttonDeleteProgram.Text = "-";
            this.buttonDeleteProgram.UseVisualStyleBackColor = true;
            // 
            // buttonDeleteSplit
            // 
            this.buttonDeleteSplit.Location = new System.Drawing.Point(1137, 392);
            this.buttonDeleteSplit.Name = "buttonDeleteSplit";
            this.buttonDeleteSplit.Size = new System.Drawing.Size(32, 32);
            this.buttonDeleteSplit.TabIndex = 18;
            this.buttonDeleteSplit.Text = "-";
            this.buttonDeleteSplit.UseVisualStyleBackColor = true;
            // 
            // buttonAddSplit
            // 
            this.buttonAddSplit.Location = new System.Drawing.Point(1137, 354);
            this.buttonAddSplit.Name = "buttonAddSplit";
            this.buttonAddSplit.Size = new System.Drawing.Size(32, 32);
            this.buttonAddSplit.TabIndex = 17;
            this.buttonAddSplit.Text = "+";
            this.buttonAddSplit.UseVisualStyleBackColor = true;
            // 
            // buttonDeleteLayer
            // 
            this.buttonDeleteLayer.Location = new System.Drawing.Point(1137, 174);
            this.buttonDeleteLayer.Name = "buttonDeleteLayer";
            this.buttonDeleteLayer.Size = new System.Drawing.Size(32, 32);
            this.buttonDeleteLayer.TabIndex = 20;
            this.buttonDeleteLayer.Text = "-";
            this.buttonDeleteLayer.UseVisualStyleBackColor = true;
            // 
            // buttonAddLayer
            // 
            this.buttonAddLayer.Location = new System.Drawing.Point(1137, 136);
            this.buttonAddLayer.Name = "buttonAddLayer";
            this.buttonAddLayer.Size = new System.Drawing.Size(32, 32);
            this.buttonAddLayer.TabIndex = 19;
            this.buttonAddLayer.Text = "+";
            this.buttonAddLayer.UseVisualStyleBackColor = true;
            // 
            // buttonProgramProperties
            // 
            this.buttonProgramProperties.Location = new System.Drawing.Point(383, 212);
            this.buttonProgramProperties.Name = "buttonProgramProperties";
            this.buttonProgramProperties.Size = new System.Drawing.Size(32, 32);
            this.buttonProgramProperties.TabIndex = 21;
            this.buttonProgramProperties.Text = "...";
            this.buttonProgramProperties.UseVisualStyleBackColor = true;
            this.buttonProgramProperties.Click += new System.EventHandler(this.buttonProgramProperties_Click);
            // 
            // buttonLayerProperties
            // 
            this.buttonLayerProperties.Location = new System.Drawing.Point(1137, 212);
            this.buttonLayerProperties.Name = "buttonLayerProperties";
            this.buttonLayerProperties.Size = new System.Drawing.Size(32, 32);
            this.buttonLayerProperties.TabIndex = 22;
            this.buttonLayerProperties.Text = "...";
            this.buttonLayerProperties.UseVisualStyleBackColor = true;
            // 
            // buttonSplitProperties
            // 
            this.buttonSplitProperties.Location = new System.Drawing.Point(1137, 430);
            this.buttonSplitProperties.Name = "buttonSplitProperties";
            this.buttonSplitProperties.Size = new System.Drawing.Size(32, 32);
            this.buttonSplitProperties.TabIndex = 23;
            this.buttonSplitProperties.Text = "...";
            this.buttonSplitProperties.UseVisualStyleBackColor = true;
            this.buttonSplitProperties.Click += new System.EventHandler(this.buttonSplitProperties_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusTextFilename,
            this.statusTextVersion,
            this.statusTextNumPrograms});
            this.statusStrip1.Location = new System.Drawing.Point(0, 762);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1176, 36);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 24;
            this.statusStrip1.Text = "statusBar";
            // 
            // statusTextFilename
            // 
            this.statusTextFilename.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusTextFilename.Name = "statusTextFilename";
            this.statusTextFilename.Size = new System.Drawing.Size(86, 29);
            this.statusTextFilename.Text = "Filename";
            // 
            // statusTextVersion
            // 
            this.statusTextVersion.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.statusTextVersion.Name = "statusTextVersion";
            this.statusTextVersion.Size = new System.Drawing.Size(105, 29);
            this.statusTextVersion.Text = "File Version";
            // 
            // statusTextNumPrograms
            // 
            this.statusTextNumPrograms.Name = "statusTextNumPrograms";
            this.statusTextNumPrograms.Size = new System.Drawing.Size(181, 29);
            this.statusTextNumPrograms.Text = "Number of Programs";
            // 
            // buttonSplitPlay
            // 
            this.buttonSplitPlay.Location = new System.Drawing.Point(1137, 468);
            this.buttonSplitPlay.Name = "buttonSplitPlay";
            this.buttonSplitPlay.Size = new System.Drawing.Size(32, 32);
            this.buttonSplitPlay.TabIndex = 25;
            this.buttonSplitPlay.Text = "▶";
            this.buttonSplitPlay.UseVisualStyleBackColor = true;
            this.buttonSplitPlay.Click += new System.EventHandler(this.buttonSplitPlay_Click);
            // 
            // contextMenuSplit
            // 
            this.contextMenuSplit.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuSplit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemPlay,
            this.toolStripSeparator2,
            this.toolStripMenuItemExportWaveform,
            this.exportRawWaveformToolStripMenuItem,
            this.toolStripSeparator1,
            this.editToolStripMenuItem,
            this.toolStripMenuItemProperties});
            this.contextMenuSplit.Name = "contextMenuSplit";
            this.contextMenuSplit.Size = new System.Drawing.Size(273, 176);
            // 
            // toolStripMenuItemPlay
            // 
            this.toolStripMenuItemPlay.Name = "toolStripMenuItemPlay";
            this.toolStripMenuItemPlay.Size = new System.Drawing.Size(272, 32);
            this.toolStripMenuItemPlay.Text = "Play Waveform";
            this.toolStripMenuItemPlay.Click += new System.EventHandler(this.toolStripMenuItemPlay_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(269, 6);
            // 
            // toolStripMenuItemExportWaveform
            // 
            this.toolStripMenuItemExportWaveform.Name = "toolStripMenuItemExportWaveform";
            this.toolStripMenuItemExportWaveform.Size = new System.Drawing.Size(272, 32);
            this.toolStripMenuItemExportWaveform.Text = "Export Waveform...";
            this.toolStripMenuItemExportWaveform.Click += new System.EventHandler(this.toolStripMenuItemExportWaveform_Click);
            // 
            // exportRawWaveformToolStripMenuItem
            // 
            this.exportRawWaveformToolStripMenuItem.Name = "exportRawWaveformToolStripMenuItem";
            this.exportRawWaveformToolStripMenuItem.Size = new System.Drawing.Size(272, 32);
            this.exportRawWaveformToolStripMenuItem.Text = "Export Raw Waveform...";
            this.exportRawWaveformToolStripMenuItem.Click += new System.EventHandler(this.exportRawWaveformToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(269, 6);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(272, 32);
            this.editToolStripMenuItem.Text = "Edit Split...";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // toolStripMenuItemProperties
            // 
            this.toolStripMenuItemProperties.Name = "toolStripMenuItemProperties";
            this.toolStripMenuItemProperties.Size = new System.Drawing.Size(272, 32);
            this.toolStripMenuItemProperties.Text = "Properties...";
            this.toolStripMenuItemProperties.Click += new System.EventHandler(this.toolStripMenuItemProperties_Click);
            // 
            // buttonSplitStop
            // 
            this.buttonSplitStop.Location = new System.Drawing.Point(1137, 506);
            this.buttonSplitStop.Name = "buttonSplitStop";
            this.buttonSplitStop.Size = new System.Drawing.Size(32, 32);
            this.buttonSplitStop.TabIndex = 26;
            this.buttonSplitStop.Text = "■";
            this.buttonSplitStop.UseVisualStyleBackColor = true;
            this.buttonSplitStop.Click += new System.EventHandler(this.buttonSplitStop_Click);
            // 
            // ProgramBankEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1176, 798);
            this.Controls.Add(this.buttonSplitStop);
            this.Controls.Add(this.buttonSplitPlay);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.buttonSplitProperties);
            this.Controls.Add(this.buttonLayerProperties);
            this.Controls.Add(this.buttonProgramProperties);
            this.Controls.Add(this.buttonDeleteLayer);
            this.Controls.Add(this.buttonAddLayer);
            this.Controls.Add(this.buttonDeleteSplit);
            this.Controls.Add(this.buttonAddSplit);
            this.Controls.Add(this.buttonDeleteProgram);
            this.Controls.Add(this.buttonAddProgram);
            this.Controls.Add(this.buttonMoveSplitDown);
            this.Controls.Add(this.buttonMoveSplitUp);
            this.Controls.Add(this.buttonMoveLayerDown);
            this.Controls.Add(this.buttonMoveLayerUp);
            this.Controls.Add(this.buttonMoveProgramDown);
            this.Controls.Add(this.buttonMoveProgramUp);
            this.Controls.Add(this.listViewSplits);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listViewLayers);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listViewPrograms);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "ProgramBankEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Program Bank Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuSplit.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewPrograms;
        private System.Windows.Forms.ColumnHeader columnProgramID;
        private System.Windows.Forms.ColumnHeader columnProgramName;
        private System.Windows.Forms.ColumnHeader columnProgramSplitsNo;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView listViewLayers;
        private System.Windows.Forms.ColumnHeader columnLayerID;
        private System.Windows.Forms.ColumnHeader columnLayerBendPlus;
        private System.Windows.Forms.ColumnHeader columnLayerDelay;
        private System.Windows.Forms.ColumnHeader columnLayerSplitNo;
        private System.Windows.Forms.ColumnHeader columnLayerSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView listViewSplits;
        private System.Windows.Forms.ColumnHeader columnSplitID;
        private System.Windows.Forms.ColumnHeader columnSplitNoteStart;
        private System.Windows.Forms.ColumnHeader columnSplitDirect;
        private System.Windows.Forms.ColumnHeader columnSplitPanpot;
        private System.Windows.Forms.ColumnHeader columnSplitFXLevel;
        private System.Windows.Forms.ColumnHeader columnSplitFXChannel;
        private System.Windows.Forms.ColumnHeader columnSplitSize;
        private System.Windows.Forms.Button buttonMoveProgramUp;
        private System.Windows.Forms.Button buttonMoveProgramDown;
        private System.Windows.Forms.Button buttonMoveLayerDown;
        private System.Windows.Forms.Button buttonMoveLayerUp;
        private System.Windows.Forms.Button buttonMoveSplitDown;
        private System.Windows.Forms.Button buttonMoveSplitUp;
        private System.Windows.Forms.Button buttonAddProgram;
        private System.Windows.Forms.Button buttonDeleteProgram;
        private System.Windows.Forms.Button buttonDeleteSplit;
        private System.Windows.Forms.Button buttonAddSplit;
        private System.Windows.Forms.Button buttonDeleteLayer;
        private System.Windows.Forms.Button buttonAddLayer;
        private System.Windows.Forms.ColumnHeader columnLayerBendMinus;
        private System.Windows.Forms.ColumnHeader columnSplitNoteEnd;
        private System.Windows.Forms.ColumnHeader columnProgramSize;
        private System.Windows.Forms.Button buttonProgramProperties;
        private System.Windows.Forms.Button buttonLayerProperties;
        private System.Windows.Forms.Button buttonSplitProperties;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusTextFilename;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSizeAsHexadecimalToolStripMenuItem;
        private System.Windows.Forms.Button buttonSplitPlay;
        private System.Windows.Forms.ContextMenuStrip contextMenuSplit;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPlay;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExportWaveform;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemProperties;
        private System.Windows.Forms.ToolStripMenuItem exportRawWaveformToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel statusTextVersion;
        private System.Windows.Forms.ToolStripStatusLabel statusTextNumPrograms;
        private System.Windows.Forms.Button buttonSplitStop;
    }
}