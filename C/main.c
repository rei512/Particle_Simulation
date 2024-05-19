#include <stdio.h>
#include <math.h>
#include <windows.h>

#define X 0
#define Y 1

#define N 4
#define GRID 8
#define SIZE 8
#define ELASTIC 0.7f
#define L (SIZE/GRID)
#define TIME_STEP 0.05f

typedef struct {
	float x;
	float y;
} Vector2;

typedef struct {
	float Radius;
	float Mass;

	Vector2 Position;
	Vector2 Velocity;

} particle;

float Pow2f(float n);
float SqrtMagnitudef(Vector2 *a);
float Magnitudef(Vector2 *a);

void Display(particle *Field[GRID+2][GRID+2]);

Vector2 Gravity;

int main(void) {
	printf("main\n");
	Gravity.x = 0.0f;
	Gravity.y = 10.0f;

	particle Particles[N];
	particle *Field[GRID+2][GRID+2];

	int k = 0;
	
	for (int i = 0; i < sqrtf(N); i++) {
		for (int j = 0; j < sqrtf(N); j++) {
			particle *This = &Particles[k++];

			This->Radius = L/1.05;
			This->Mass = 1.0f;
			This->Velocity.x = 0.0f;
			This->Velocity.y = 0.0f;

			This->Position.x = -2.0f + 4.0f*j;
			This->Position.y = -2.0f + 4.0f*i;
		}
	}
	/*
	particle *This = &Particles[0];

	This->Radius = L/sqrtf(2);
	This->Mass = 1.0f;
	This->Velocity.x = 5.0f;
	This->Velocity.y = 0.0f;
	This->Position.x = -2.0f;
	This->Position.y = 0.0f;

	This = &Particles[1];

	This->Radius = L/sqrtf(2);
	This->Mass = 1.0f;
	This->Velocity.x = 0.0f;
	This->Velocity.y = 0.0f;
	This->Position.x = 2.0f;
	This->Position.y = 0.0f;
*/


	printf("initialize\n");
	while(1) {
		for(int i=0;i<GRID+2;i++)
			for(int j=0;j<GRID+2;j++)
				Field[i][j] = NULL;
		
		//printf("Field initialize\n");


		for(int i=0;i<N;i++) {
			particle *This = &Particles[i];

			Vector2 Index = {(This->Position.x + SIZE/2.0f) / L, (This->Position.y + SIZE/2.0f) / L};
			/*
			if(Index.x > GRID-1 || Index.x < 0.0f)
				This->Position.x = 0.0f;
			if(Index.y > GRID-1 || Index.y < 0.0f)
				This->Position.y = 0.0f;
			*/
			Field[(int)Index.x][(int)Index.y] = This;
		}

		//printf("Field set\n");

		for (int i = 0; i < GRID; i++) {
			for (int j = 0; j < GRID; j++) {
				//printf("a\n");

				if(Field[i][j] != NULL) {
					particle *This = Field[i][j];

					//printf("b\n");
					This->Velocity.x += Gravity.x * TIME_STEP;
					//printf("c\n");
					This->Velocity.y += Gravity.y * TIME_STEP;
					//printf("d\n");

					if(fabs(This->Position.x + This->Velocity.x * TIME_STEP) >= SIZE/2.0f - This->Radius) {
						This->Velocity.x *= -1.0f * ELASTIC;
						This->Velocity.y *= (1.0f - (1.0f - ELASTIC) * 0.1f);
					}
					//printf("e\n");

					if(fabs(This->Position.y + This->Velocity.y * TIME_STEP) >= SIZE/2.0f - This->Radius) {
						This->Velocity.y *= -1.0f * ELASTIC;
						This->Velocity.x *= (1.0f - (1.0f - ELASTIC) * 0.1f);
					}


					for(int k=-2;k<=2;k++) {
						for(int u=-2;u<=2;u++) {
							if (u > 0 || (u==0 && k>=0))
								break;

							if (i+k >= 0 && j+u >= 0 && Field[i + k][j + u] != NULL) {
								particle *Another = Field[i+k][j+u];

								Vector2 r = {Another->Position.x + Another->Velocity.x * TIME_STEP - This->Position.x - This->Velocity.x * TIME_STEP, Another->Position.y + Another->Velocity.y * TIME_STEP - This->Position.y - This->Velocity.y * TIME_STEP};

								if(SqrtMagnitudef(&r) <= Pow2f(This->Radius + Another->Radius)) {
									Vector2 v = {This->Velocity.x - Another->Velocity.x, This->Velocity.y - Another->Velocity.y};
									float theta = atan2f(r.y, r.x);
									float alpha = atan2f(v.y, v.x) - theta;

									Vector2 v1 = {Magnitudef(&v) * cosf(alpha), Magnitudef(&v) * sinf(alpha)};

									Vector2 vi = {(1.0f + ELASTIC) * Another->Mass / (This->Mass + Another->Mass) * v1.x, 0.0f};
									Vector2 vj = {(1.0f + ELASTIC) * This->Mass / (This->Mass + Another->Mass) * v1.x, 0.0f};

									This->Velocity.x -= vi.x * cosf(theta);
									This->Velocity.y -= vi.x * sinf(theta);

									Another->Velocity.x += vj.x * cosf(theta);
									Another->Velocity.y += vj.x * sinf(theta);
								}
							}
						}
					}
				}
			}
		}

		//printf("Collider detected\n");

		for (int i = 0; i < N; i++) {
			particle *This = &Particles[i];

			This->Position.x += This->Velocity.x * TIME_STEP;
			This->Position.y += This->Velocity.y * TIME_STEP;
		}

		//printf("Finish\n");

		Display(Field);
	}
	return 0;
}

float Pow2f(float n) {
	return n*n;
}

float SqrtMagnitudef(Vector2 *a) {
	return Pow2f(a->x) + Pow2f(a->y);
}

float Magnitudef(Vector2 *a) {
	return sqrtf(SqrtMagnitudef(a));
}

void Display(particle *Field[GRID+2][GRID+2]) {
	system("cls");

	for (int i = 0; i < GRID+1; i++)
		printf("--");
	printf("\n");

	for (int i = 0; i < GRID; i++) {
		printf("|");
		for (int j = 0; j < GRID; j++) {
			if(Field[j][i] != NULL)
				printf("■");
			else
				printf("  ");
		}
		printf("|\n");
	}

	for (int i = 0; i < GRID+1; i++)
		printf("--");
	printf("\n");
	//printf("Displayed\n");
}