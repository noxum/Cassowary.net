/*
  Cassowary.net: an incremental constraint solver for .NET
  (http://lumumba.uhasselt.be/jo/projects/cassowary.net/)
  
  Copyright (C) 2005-2006  Jo Vermeulen (jo.vermeulen@uhasselt.be)
  
  This program is free software; you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public License
  as published by the Free Software Foundation; either version 2.1
  of  the License, or (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Lesser General Public License for more details.

  You should have received a copy of the GNU Lesser General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

namespace Cassowary
{
    public class ClLinearInequality : ClLinearConstraint
    {
        public ClLinearInequality(ClLinearExpression cle, ClStrength strength, double weight)
            : base(cle, strength, weight)
        {
        }

        public ClLinearInequality(ClLinearExpression cle, ClStrength strength)
            : base(cle, strength)
        {
        }

        public ClLinearInequality(ClLinearExpression cle)
            : base(cle)
        {
        }

        public ClLinearInequality(ClVariable clv1, Cl.Operator op, ClVariable clv2, ClStrength strength, double weight = 1.0)
            : base(new ClLinearExpression(clv2), strength, weight)
            /* throws ExClInternalError */
        {
            switch (op)
            {
                case Cl.Operator.GreaterThanOrEqualTo:
                    _expression.MultiplyMe(-1.0);
                    _expression.AddVariable(clv1);
                    break;
                case Cl.Operator.LessThanOrEqualTo:
                    _expression.AddVariable(clv1, -1.0);
                    break;
                default:
                    // invalid operator
                    throw new CassowaryInternalException("Invalid operator in ClLinearInequality constructor");
            }
        }

        public ClLinearInequality(ClVariable clv1, Cl.Operator op, ClVariable clv2)
            : this(clv1, op, clv2, ClStrength.Required, 1.0)
            /* throws ExClInternalError */
        {
        }

        public ClLinearInequality(ClVariable clv, Cl.Operator op, double val, ClStrength strength, double weight = 1.0)
            : base(new ClLinearExpression(val), strength, weight)
            /* throws ExClInternalError */
        {
            switch (op)
            {
                case Cl.Operator.GreaterThanOrEqualTo:
                    _expression.MultiplyMe(-1.0);
                    _expression.AddVariable(clv);
                    break;
                case Cl.Operator.LessThanOrEqualTo:
                    _expression.AddVariable(clv, -1.0);
                    break;
                default:
                    // invalid operator
                    throw new CassowaryInternalException("Invalid operator in ClLinearInequality constructor");
            }
        }

        public ClLinearInequality(ClVariable clv, Cl.Operator op, double val)
            : this(clv, op, val, ClStrength.Required, 1.0)
            /* throws ExClInternalError */
        {
        }

        public ClLinearInequality(ClLinearExpression cle1, Cl.Operator op, ClLinearExpression cle2, ClStrength strength, double weight = 1.0)
            : base(cle2.Clone(), strength, weight)
            /* throws ExClInternalError */
        {
            switch (op)
            {
                case Cl.Operator.GreaterThanOrEqualTo:
                    _expression.MultiplyMe(-1.0);
                    _expression.AddExpression(cle1);
                    break;
                case Cl.Operator.LessThanOrEqualTo:
                    _expression.AddExpression(cle1, -1.0);
                    break;
                default:
                    // invalid operator
                    throw new CassowaryInternalException("Invalid operator in ClLinearInequality constructor");
            }
        }

        public ClLinearInequality(ClLinearExpression cle1, Cl.Operator op, ClLinearExpression cle2)
            : this(cle1, op, cle2, ClStrength.Required, 1.0)
            /* throws ExClInternalError */
        {
        }

        public ClLinearInequality(ClAbstractVariable clv, Cl.Operator op, ClLinearExpression cle, ClStrength strength, double weight = 1.0)
            : base(cle.Clone(), strength, weight)
            /* throws ExClInternalError */
        {
            switch (op)
            {
                case Cl.Operator.GreaterThanOrEqualTo:
                    _expression.MultiplyMe(-1.0);
                    _expression.AddVariable(clv);
                    break;
                case Cl.Operator.LessThanOrEqualTo:
                    _expression.AddVariable(clv, -1.0);
                    break;
                default:
                    // invalid operator
                    throw new CassowaryInternalException("Invalid operator in ClLinearInequality constructor");
            }
        }

        public ClLinearInequality(ClAbstractVariable clv, Cl.Operator op, ClLinearExpression cle)
            : this(clv, op, cle, ClStrength.Required, 1.0)
            /* throws ExClInternalError */
        {
        }

        public ClLinearInequality(ClLinearExpression cle, Cl.Operator op, ClAbstractVariable clv, ClStrength strength, double weight = 1.0)
            : base(cle.Clone(), strength, weight)
            /* throws ExClInternalError */
        {
            switch (op)
            {
                case Cl.Operator.LessThanOrEqualTo:
                    _expression.MultiplyMe(-1.0);
                    _expression.AddVariable(clv);
                    break;
                case Cl.Operator.GreaterThanOrEqualTo:
                    _expression.AddVariable(clv, -1.0);
                    break;
                default:
                    // invalid operator
                    throw new CassowaryInternalException("Invalid operator in ClLinearInequality constructor");
            }
        }

        public ClLinearInequality(ClLinearExpression cle, Cl.Operator op, ClAbstractVariable clv)
            : this(cle, op, clv, ClStrength.Required, 1.0)
            /* throws ExClInternalError */
        {
        }

        public override sealed bool IsInequality
        {
            get { return true; }
        }

        public override sealed string ToString()
        {
            return base.ToString() + " >= 0)";
        }
    }
}