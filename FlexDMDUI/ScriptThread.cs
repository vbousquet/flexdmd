using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using EXCEPINFO = System.Runtime.InteropServices.ComTypes.EXCEPINFO;

namespace FlexDMDUI
{
    class ScriptThread : IActiveScriptSite
    {
        private IActiveScript _scriptEngine;
        private VBScriptEngine _scriptEngineObject;
        private ActiveScriptParseWrapper _scriptParser;
        private Thread _processThread = null;
        private int _running = 0;
        private readonly List<string> _runnables = new List<string>();

        UInt32 SCRIPTTHREADID_CURRENT = 0xFFFFFFFD;
        UInt32 SCRIPTTHREADID_BASE = 0xFFFFFFFE;
        UInt32 SCRIPTTHREADID_ALL = 0xFFFFFFFF;

        public ScriptThread(string name = "ScriptThread")
        {
            _running = 1;
            _processThread = new Thread(new ThreadStart(ScriptFunc));
            _processThread.Name = name;
            _processThread.IsBackground = true;
            _processThread.Start();
        }

        public void Close(bool complete, bool wait)
        {
            _running = complete ? 2 : 0;
            if (wait) _processThread.Join();
            Console.WriteLine("Thread " + _processThread.Name + " closure requested.");
        }

        public void Interrupt()
        {
            // See http://e.craft.free.fr/ActiveScriptingLostFAQ/hostrun.htm
            // And https://github.com/jango2015/VS-Macros/tree/master/ExecutionEngine/ActiveScript%20Interfaces
            if (IsRunning())
            {
                try
                {
                    EXCEPINFO ei = new EXCEPINFO
                    {
                        wCode = 1000,
                        scode = 0,
                        bstrSource = "Interrupt",
                        bstrDescription = "Interrupt",
                        bstrHelpFile = "Interrupt",
                        pfnDeferredFillIn = IntPtr.Zero,
                        dwHelpContext = 0,
                        wReserved = 0,
                        pvReserved = IntPtr.Zero,
                    };
                    _scriptEngine.InterruptScriptThread(SCRIPTTHREADID_BASE, ref ei, 0);
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("Exception while interrupting thread '{0}'\nException: {1}", _processThread.Name, e));
                }
            }
        }

        public void Post(string code)
        {
            lock (_runnables)
            {
                _runnables.Add(code);
            }
        }

        public bool IsRunning()
        {
            if (_scriptEngine == null) return false;
            _scriptEngine.GetScriptThreadState(SCRIPTTHREADID_BASE, out ScriptThreadState state);
            return state == ScriptThreadState.Running;
        }

        private void ScriptFunc()
        {
            _scriptEngineObject = new VBScriptEngine();
            _scriptEngine = (IActiveScript)_scriptEngineObject;
            _scriptParser = new ActiveScriptParseWrapper(_scriptEngine);
            _scriptParser.InitNew();
            _scriptEngine.SetScriptSite(this);
            _scriptEngine.SetScriptState(ScriptState.Started);
            _scriptEngine.SetScriptState(ScriptState.Connected);
            while (true)
            {
                if (_running == 0) break;
                string code = "";
                lock (_runnables)
                {
                    _runnables.ForEach(snippet => code += snippet + "\n");
                    _runnables.Clear();
                }
                if (code.Trim().Length > 0)
                {
                    try
                    {
                        /*_scriptEngine.GetScriptThreadState(SCRIPTTHREADID_BASE, out ScriptThreadState state);
                        Console.WriteLine(string.Format("Scipt thread state for thread '{0}' is {1}", _processThread.Name, state));
                        _scriptEngine.GetScriptState(out ScriptState sciptState);
                        Console.WriteLine(string.Format("Scipt state for thread '{0}' is {1}", _processThread.Name, sciptState));*/
                        _scriptParser.ParseScriptText(code, null, null, null, IntPtr.Zero, 0, ScriptText.IsVisible, out object result, out EXCEPINFO ei);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(string.Format("Exception while parsing following code for thread '{0}'\nException: {1}\nCode:\n{2}", _processThread.Name, e, code));
                    }
                }
                else if (_running == 2) break;
                Thread.Sleep(100);
            }
            _scriptEngine.Close();
            Marshal.ReleaseComObject(_scriptEngine);
            Marshal.ReleaseComObject(_scriptEngineObject);
            _scriptParser.ReleaseComObject();
            _scriptEngine = null;
            _scriptParser = null;
        }

        #region Implementation of IActiveScriptSite

        void IActiveScriptSite.GetLCID(out int lcid)
        {
            lcid = Thread.CurrentThread.CurrentUICulture.LCID;
        }

        void IActiveScriptSite.GetItemInfo(string name, ScriptInfo returnMask, object[] item, IntPtr[] typeInfo)
        {
            if (name == null)
            {
                throw new ArgumentNullException(@"name");
            }

            if (name != @"Context")
            {
                throw new COMException(null, (int)HRESULT.TYPE_E_ELEMENTNOTFOUND);
            }

            if ((returnMask & ScriptInfo.IUnknown) != 0)
            {
                if (item == null)
                {
                    throw new ArgumentNullException(@"item");
                }

                item[0] = this;
            }

            if ((returnMask & ScriptInfo.ITypeInfo) != 0)
            {
                if (typeInfo == null)
                {
                    throw new ArgumentNullException(@"typeInfo");
                }

                typeInfo[0] = IntPtr.Zero;
            }
        }

        void IActiveScriptSite.GetDocVersionString(out string version)
        {
            throw new NotImplementedException();
        }

        public void OnScriptTerminate(object result, EXCEPINFO exceptionInfo)
        {
        }

        void IActiveScriptSite.OnStateChange(ScriptState scriptState)
        {
        }

        void IActiveScriptSite.OnScriptError(IActiveScriptError scriptError)
        {
            scriptError.GetExceptionInfo(out EXCEPINFO exceptionInfo);
            scriptError.GetSourcePosition(out uint sourceContext, out uint lineNumber, out int characterPosition);
            scriptError.GetSourceLineText(out string sourceLine);
            var msg = string.Format("'{0}' at line {1}, character {2}:\n\n{3}", exceptionInfo.bstrDescription, lineNumber - 4, characterPosition, sourceLine);
            System.Windows.Forms.MessageBox.Show(msg, exceptionInfo.bstrSource);
        }

        void IActiveScriptSite.OnEnterScript()
        {
        }

        void IActiveScriptSite.OnLeaveScript()
        {
        }

        #endregion

    }
}
