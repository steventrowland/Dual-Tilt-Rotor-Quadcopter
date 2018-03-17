#pragma once

#include <Quadcopter.h>
#include <Vector.h>
#include <Rotation.h>
#include <Thruster.h>

using namespace System;

namespace DTRQCSInterface {
	struct SVector {
		double X;
		double Y;
		double Z;

		SVector(){}

		SVector(Vector3D v) {
			this->X = v.X;
			this->Y = v.Y;
			this->Z = v.Z;
		}

		SVector(double X, double Y, double Z) {
			this->X = X;
			this->Y = Y;
			this->Z = Z;
		}
	};

	struct SDirAngle {
		double Rotation;
		SVector Direction;

		SDirAngle(){}

		SDirAngle(DirectionAngle dA) {
			this->Rotation = dA.Rotation;
			this->Direction = SVector(dA.Direction);
		}

		SDirAngle(double Rotation, SVector Direction) {
			this->Rotation = Rotation;
			this->Direction = Direction;
		}
	};

	struct SThrust {
		SVector CurrentPosition;
		SVector CurrentRotation;
		SVector TargetPosition;

		SThrust(){}

		SThrust(SVector CurrentPosition, SVector CurrentRotation, SVector TargetPosition) {
			this->CurrentPosition = CurrentPosition;
			this->CurrentRotation = CurrentRotation;
			this->TargetPosition = TargetPosition;
		}
	};

	struct SQuad {
		SVector CurrentPosition;
		SDirAngle CurrentRotation;
		SVector TargetPosition;
		SThrust TB, TC, TD, TE;

		SQuad(){}

		SQuad(SVector CurrentPosition, SDirAngle CurrentRotation, SVector TargetPosition, SThrust TB, SThrust TC, SThrust TD, SThrust TE) {
			this->CurrentPosition = CurrentPosition;
			this->CurrentRotation = CurrentRotation;
			this->TargetPosition = TargetPosition;
			this->TB = TB;
			this->TC = TC;
			this->TD = TD;
			this->TE = TE;
		}
	};


	public ref class DTRQQuadcopter
	{
	private:
		Quadcopter* quadcopter;
	public:
		DTRQQuadcopter() : quadcopter(new Quadcopter(true, 0.3, 55, 0.05)) {}
		~DTRQQuadcopter() { delete quadcopter; }

		void CalculateCombinedThrustVector() {
			quadcopter->CalculateCombinedThrustVector();
		}

		void SetTarget(double pos[3], double dA[4]) {
			quadcopter->SetTarget(Vector3D(pos[0], pos[1], pos[2]), Rotation(DirectionAngle(dA[0], Vector3D(dA[1], dA[2], dA[3]))));
		}

		void SimulateCurrent(double extAcc[3]) {
			quadcopter->SimulateCurrent(Vector3D(extAcc[0], extAcc[1], extAcc[2]));
		}

		SQuad GetQuadcopter() {
			return SQuad{
				SVector(quadcopter->CurrentPosition),
				SDirAngle(quadcopter->CurrentRotation.GetDirectionAngle()),
				SVector(quadcopter->TargetPosition),
				SThrust(quadcopter->TB.CurrentPosition, quadcopter->TB.CurrentRotation, quadcopter->TB.TargetPosition),
				SThrust(quadcopter->TC.CurrentPosition, quadcopter->TC.CurrentRotation, quadcopter->TC.TargetPosition),
				SThrust(quadcopter->TD.CurrentPosition, quadcopter->TD.CurrentRotation, quadcopter->TD.TargetPosition),
				SThrust(quadcopter->TE.CurrentPosition, quadcopter->TE.CurrentRotation, quadcopter->TE.TargetPosition)
			};
		}

	};
}
