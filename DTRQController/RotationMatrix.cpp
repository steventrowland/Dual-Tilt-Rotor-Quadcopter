#include "RotationMatrix.h"

RotationMatrix::RotationMatrix(Vector3D axes) {
	XAxis = Vector3D(axes.X, axes.X, axes.X);
	YAxis = Vector3D(axes.Y, axes.Y, axes.Y);
	ZAxis = Vector3D(axes.Z, axes.Z, axes.Z);
}

RotationMatrix::RotationMatrix(Vector3D X, Vector3D Y, Vector3D Z) {
	XAxis = X;
	YAxis = Y;
	ZAxis = Z;
}

Vector3D RotationMatrix::ConvertCoordinateToVector() {
	if (didRotate)
	{
		return Vector3D((XAxis.X + YAxis.X + ZAxis.X), (XAxis.Y + YAxis.Y + ZAxis.Y), (XAxis.Z + YAxis.Z + ZAxis.Z));
	}
	else
	{
		return InitialVector;
	}
}

void RotationMatrix::ReadjustMatrix() {
	double X = (XAxis.X + YAxis.X + ZAxis.X);
	double Y = (XAxis.Y + YAxis.Y + ZAxis.Y);
	double Z = (XAxis.Z + YAxis.Z + ZAxis.Z);

	XAxis = Vector3D(X, X, X);
	YAxis = Vector3D(Y, Y, Y);
	ZAxis = Vector3D(Z, Z, Z);
}

void RotationMatrix::Rotate(Vector3D rotation) {
	if (rotation.X != 0)
	{
		RotateX(rotation.X);
		didRotate = true;

		if (rotation.Y != 0 || rotation.Z != 0)
		{
			ReadjustMatrix();
		}
	}

	if (rotation.Y != 0)
	{
		RotateY(rotation.Y);
		didRotate = true;

		if (rotation.Z != 0)
		{
			ReadjustMatrix();
		}
	}

	if (rotation.Z != 0)
	{
		RotateZ(rotation.Z);
		didRotate = true;
	}
}

void RotationMatrix::RotateX(double theta) {
	double c = cos(Mathematics::DegreesToRadians(theta));
	double s = sin(Mathematics::DegreesToRadians(theta));

	XAxis = Vector3D(1, 0,  0).Multiply(XAxis);
	YAxis = Vector3D(0, c, -s).Multiply(YAxis);
	ZAxis = Vector3D(0, s,  c).Multiply(ZAxis);
}

void RotationMatrix::RotateY(double theta) {
	double c = cos(Mathematics::DegreesToRadians(theta));
	double s = sin(Mathematics::DegreesToRadians(theta));

	XAxis = Vector3D( c, 0, s).Multiply(XAxis);
	YAxis = Vector3D( 0, 1, 0).Multiply(YAxis);
	ZAxis = Vector3D(-s, 0, c).Multiply(ZAxis);
}

void RotationMatrix::RotateZ(double theta) {
	double c = cos(Mathematics::DegreesToRadians(theta));
	double s = sin(Mathematics::DegreesToRadians(theta));

	XAxis = Vector3D(c, -s, 0).Multiply(XAxis);
	YAxis = Vector3D(s,  c, 0).Multiply(YAxis);
	ZAxis = Vector3D(0,  0, 1).Multiply(ZAxis);
}

void RotationMatrix::Multiply(double d) {
	XAxis = XAxis.Multiply(d);
	YAxis = YAxis.Multiply(d);
	ZAxis = ZAxis.Multiply(d);
}

void RotationMatrix::Multiply(RotationMatrix rM) {
	XAxis = XAxis.Multiply(rM.XAxis);
	YAxis = YAxis.Multiply(rM.YAxis);
	ZAxis = ZAxis.Multiply(rM.ZAxis);
}

void RotationMatrix::RotateRelative(RotationMatrix rM) {
	Multiply(rM);
}

void RotationMatrix::Normalize() {
	Vector3D vz = Vector3D::CrossProduct(XAxis, YAxis);
	Vector3D vy = Vector3D::CrossProduct(vz, XAxis);

	XAxis = XAxis.UnitSphere();
	YAxis = vy.UnitSphere();
	ZAxis = vz.UnitSphere();
}

void RotationMatrix::Transpose() {
	XAxis = Vector3D(XAxis.X, YAxis.X, ZAxis.X);
	YAxis = Vector3D(XAxis.Y, YAxis.Y, ZAxis.Y);
	ZAxis = Vector3D(XAxis.Z, YAxis.Z, ZAxis.Z);
}

void RotationMatrix::Inverse() {
	XAxis = Vector3D::CrossProduct(YAxis, ZAxis);
	YAxis = Vector3D::CrossProduct(ZAxis, XAxis);
	ZAxis = Vector3D::CrossProduct(XAxis, YAxis);

	Transpose();
	Multiply(1 / Determinant());
}

bool RotationMatrix::IsEqual(RotationMatrix rM) {
	return XAxis.IsEqual(rM.XAxis) && YAxis.IsEqual(rM.YAxis) && ZAxis.IsEqual(rM.ZAxis);
}

double RotationMatrix::Determinant() {
	return XAxis.X * (YAxis.Y * ZAxis.Z - ZAxis.Y * YAxis.Z) -
		YAxis.X * (ZAxis.Z * XAxis.Y - ZAxis.Y * XAxis.Z) +
		ZAxis.X * (XAxis.Y * YAxis.Z - YAxis.Y * XAxis.Z);
}

Vector3D RotationMatrix::RotateVector(Vector3D rotate, Vector3D coordinates) {
	//calculate rotation matrix
	RotationMatrix matrix = RotationMatrix(coordinates);

	matrix.Rotate(rotate);


	if (rotate.X == 0 && rotate.Y == 0 && rotate.Z == 0) {
		return coordinates;
	}
	else {
		return matrix.ConvertCoordinateToVector();
	}
}

std::string RotationMatrix::ToString() {
	std::string x = XAxis.ToString();
	std::string y = YAxis.ToString();
	std::string z = ZAxis.ToString();

	return x + "\n" + y + "\n" + z + "\n";
}
