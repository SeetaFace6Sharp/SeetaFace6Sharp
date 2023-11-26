using System;
using System.Collections.Generic;
using System.Text;

namespace SeetaFace6Sharp.Models
{
    internal class TargetInstructionInfo
    {
        public string TargetTennisName { get; set; }

        public X86Instruction TargetX86Instruction { get; set; }

        public TargetInstructionInfo(string targetTennisName, X86Instruction instruction)
        {
            this.TargetX86Instruction = instruction;
            this.TargetTennisName = targetTennisName;
        }
    }
}
