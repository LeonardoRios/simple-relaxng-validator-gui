﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace relax_ng
{
    public partial class Form1 : Form
    {
        private OpenFileDialog _instanceFileDialog;
        private OpenFileDialog _grammarFileDialog;
        private Validator _validator;

        public Form1()
        {
            InitializeComponent();
            _instanceFileDialog = new OpenFileDialog();
            _grammarFileDialog = new OpenFileDialog();
            _validator = new Validator();
        }

        private void _loadFile(OpenFileDialog dialog, TextBox target, TextBox status, Button loadButton, Button unloadButton)
        {
            if (!String.IsNullOrEmpty(dialog.FileName))
            {
                AnnotatedFileReader reader = new AnnotatedFileReader(dialog.FileName);
                target.ReadOnly = true;
                target.Text = reader.Read();
                loadButton.Enabled = false;
                unloadButton.Enabled = true;
                txtOutput.Text = "Loaded file: " + dialog.FileName;
                txtOutput.Text += "\r\nIMPORTANT: There's no need to reload the file if it changes on disk, it will be automatically reloaded each time you press Validate! :)";
                status.Text = dialog.FileName;
            }
        }

        private void _unloadFile(OpenFileDialog dialog, TextBox target, TextBox status, Button loadButton, Button unloadButton)
        {
            txtOutput.Text = "Unloaded file: " + dialog.FileName;
            dialog.FileName = null;
            target.ReadOnly = false;
            target.Clear();
            loadButton.Enabled = true;
            unloadButton.Enabled = false;
            status.Text = "FREE EDITING";
        }

        private void _editFile(OpenFileDialog dialog, TextBox target, TextBox status, Button loadButton, Button unloadButton)
        {
            target.ReadOnly = false;
            txtOutput.Text = "Opened contents of file for editing: " + dialog.FileName;
            txtOutput.Text += "IMPORTANT: File will not be reloaded when pressing Validate!";
            dialog.FileName = null;
            loadButton.Enabled = true;
            unloadButton.Enabled = false;
            status.Text = "FREE EDITING";
        }


        /*
         * event listeners
         */

        private void btnValidate_Click(object sender, EventArgs e)
        {
            if(_instanceFileDialog.FileName != null)
                _loadFile(_instanceFileDialog, txtInstance, txtInstanceState, btnBrowseInstance, btnRemoveInstanceFile);

            if (_grammarFileDialog.FileName != null)
                _loadFile(_grammarFileDialog, txtGrammar, txtGrammarState, btnBrowseGrammar, btnRemoveGrammarFile);

            _validator.SetInstance(txtInstance.Text);
            _validator.SetGrammar(txtGrammar.Text);

            if(!_validator.HasPatternMatchError)
                btnValidate.BackColor = Color.GreenYellow;
            else
                btnValidate.BackColor = Color.IndianRed;

            txtOutput.Text = _validator.PatternMatchError;
        }

        private void btnBrowseInstance_Click(object sender, EventArgs e)
        {
            _instanceFileDialog.ShowDialog();
            _loadFile(_instanceFileDialog, txtInstance, txtInstanceState, btnBrowseInstance, btnRemoveInstanceFile);
        }

        private void btnBrowseGrammar_Click(object sender, EventArgs e)
        {
            _grammarFileDialog.ShowDialog();
            _loadFile(_grammarFileDialog, txtGrammar, txtGrammarState, btnBrowseGrammar, btnRemoveGrammarFile);
        }

        private void txtInstance_DoubleClick(object sender, EventArgs e)
        {
            if (txtInstance.ReadOnly == true)
                _editFile(_instanceFileDialog, txtInstance, txtInstanceState, btnBrowseInstance, btnRemoveInstanceFile);
        }

        private void txtGrammar_DoubleClick(object sender, EventArgs e)
        {
            if (txtGrammar.ReadOnly == true)
                _editFile(_grammarFileDialog, txtGrammar, txtGrammarState, btnBrowseGrammar, btnRemoveGrammarFile);
        }

        private void btnRemoveInstanceFile_Click(object sender, EventArgs e)
        {
            _unloadFile(_instanceFileDialog, txtInstance, txtInstanceState, btnBrowseInstance, btnRemoveInstanceFile);
        }

        private void btnRemoveGrammarFile_Click(object sender, EventArgs e)
        {
            _unloadFile(_grammarFileDialog, txtGrammar, txtGrammarState, btnBrowseGrammar, btnRemoveGrammarFile);
        }

        private void txtInstance_TextChanged(object sender, EventArgs e)
        {
            _validator.SetInstance(txtInstance.Text);
            showInstanceErrors();
            showMatchingErrors();
        }

        private void txtGrammar_TextChanged(object sender, EventArgs e)
        {
            _validator.SetGrammar(txtGrammar.Text);
            showGrammarErrors();
            showMatchingErrors();
        }

        private void showInstanceErrors()
        {
            showError(!_validator.HasInstanceFormatError, _validator.InstanceFormatError, txtInstanceState);
        }

        private void showGrammarErrors()
        {
            showError(!_validator.HasAnyGrammarError, _validator.FirstGrammarError, txtGrammarState);
        }

        private void showMatchingErrors()
        {
            if(!_validator.HasAnyGrammarError && !_validator.HasInstanceFormatError)
                showError(!_validator.HasPatternMatchError, "SUCCESSFULLY MATCHED", txtOutput);
            else if(!_validator.HasPatternMatchError)
                showError(!_validator.HasPatternMatchError, _validator.PatternMatchError, txtOutput);
        }

        private void showError(bool erronous, string message, Control target)
        {
            if (erronous)
                target.BackColor = Color.GreenYellow;
            else
                target.BackColor = Color.IndianRed;

            txtOutput.Text = message;
        }
    }
}
