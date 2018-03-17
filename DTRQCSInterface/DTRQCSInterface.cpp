#include "stdafx.h"

#include "DTRQCSInterface.h"
#include <Quadcopter.h>
#include <Vector.h>
#include <Rotation.h>
#include <Thruster.h>

#include <SVector.h>
#include <SDirAngle.h>
#include <SThrust.h>
#include <SQuad.h>

using namespace System;

namespace DTRQCSInterface {
	public ref class DTRQQuadcopter {
	private:
		Quadcopter* quadcopter;
	public:
		DTRQQuadcopter() : quadcopter(new Quadcopter(true, 0.3, 55, 0.05)) {}
		~DTRQQuadcopter() { delete quadcopter; }

		DTRQQuadcopter(bool simulation, double armLength, double armAngle, double dT) {
			quadcopter = new Quadcopter(simulation, armLength, armAngle, dT);
		}

		void CalculateCombinedThrustVector() {
			quadcopter->CalculateCombinedThrustVector();
		}

		void SetTarget(SVector^ pos, SDirAngle^ dA) {
			quadcopter->SetTarget(Vector3D(pos->X, pos->Y, pos->Z), Rotation(DirectionAngle(dA->Rotation, Vector3D(dA->Direction.X, dA->Direction.Y, dA->Direction.Z))));
		}

		void SimulateCurrent(SVector^ extAcc) {
			quadcopter->SimulateCurrent(Vector3D(extAcc->X, extAcc->Y, extAcc->Z));
		}

		SQuad^ GetQuadcopter() {
			return gcnew SQuad{
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
