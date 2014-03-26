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
    public class ClPoint
    {
        public ClPoint(double x, double y)
        {
            _clvX = new ClVariable(x);
            _clvY = new ClVariable(y);
        }

        public ClPoint(double x, double y, int a)
        {
            _clvX = new ClVariable("x" + a, x);
            _clvY = new ClVariable("y" + a, y);
        }

        public ClPoint(ClVariable clvX, ClVariable clvY)
        {
            _clvX = clvX;
            _clvY = clvY;
        }

        private ClVariable _clvX;

        public ClVariable X
        {
            get { return _clvX; }
            set { _clvX = value; }
        }

        private ClVariable _clvY;

        public ClVariable Y
        {
            get { return _clvY; }
            set { _clvY = value; }
        }

        public override string ToString()
        {
            return "(" + _clvX + ", " + _clvY + ")";
        }
    }
}