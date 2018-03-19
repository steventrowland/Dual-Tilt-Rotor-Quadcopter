#include <Thruster.h>

Thruster::Thruster() {
	this->ThrusterOffset = Vector3D(0, 0, 0);
	this->name = "null";
	this->simulation = true;
	this->dT = 0.05;

	this->CurrentPosition = Vector3D(0, 0, 0);
	this->TargetPosition = Vector3D(0, 0, 0);
	this->CurrentRotation = Vector3D(0, 0, 0);
	this->disable = false;
}

Thruster::Thruster(Vector3D thrusterOffset, std::string name, bool simulation, double dT) {
	this->ThrusterOffset = thrusterOffset;
	this->name = name;
	this->simulation = simulation;
	this->dT = dT;

	this->CurrentPosition = Vector3D(0, 0, 0);
	this->TargetPosition = Vector3D(0, 0, 0);
	this->CurrentRotation = Vector3D(0, 0, 0);
	this->disable = false;
	
	if (simulation) {
		std::cout << "  Thruster initializing in simulation mode." << std::endl;

		this->outerCDS = CriticallyDampedSpring(dT, 150, 1, "Thruster " + this->name + " outer");
		this->innerCDS = CriticallyDampedSpring(dT, 250, 1, "Thruster " + this->name + " inner");
		this->rotorCDS = CriticallyDampedSpring(dT, 150, 1, "Thruster " + this->name + " rotor");
	}

	std::cout << "  Thruster " << name << ": Offset:" << thrusterOffset.ToString() << " Simulation: " << simulation << " dT:" << dT << std::endl;
}

Vector3D Thruster::ReturnThrustVector() {
	Vector3D thrustVector = Vector3D(0, rotor.GetOutput(), 0);
	Vector3D rotationVector = Vector3D(-outerJoint.GetAngle(), 0, innerJoint.GetAngle());

	thrustVector = RotationMatrix::RotateVector(rotationVector, thrustVector);

	return thrustVector;
}

Vector3D Thruster::ReturnThrusterOutput() {
	return Vector3D(outerJoint.GetAngle(), rotor.GetOutput(), innerJoint.GetAngle());
}

void Thruster::SetThrusterOutputs(Vector3D output) {
	//Disable negative thrust output
	CheckIfDisabled();

	output.X = disable ? 0 : output.X;
	output.Y = disable ? 0 : output.Y;
	output.Z = disable ? 0 : output.Z;

	//Sets current rotation of thruster for use in the visualization of the quad
	CurrentRotation = Vector3D(-outerJoint.GetAngle(), 0, -innerJoint.GetAngle());

	//Sets the outputs of the thrusters
	if (simulation) {
		innerJoint.SetAngle(innerCDS.Calculate(output.X));
		rotor.SetOutput(rotorCDS.Calculate(output.Y));
		outerJoint.SetAngle(outerCDS.Calculate(output.Z));
	}
	else {
		innerJoint.SetAngle(output.X);
		rotor.SetOutput(output.Y);
		outerJoint.SetAngle(output.Z);
	}
}

bool Thruster::CheckIfDisabled() {
	//setDisabled if dshot
	return rotor.CheckESC();
}

bool Thruster::IsDisabled() {
	return disable;
}