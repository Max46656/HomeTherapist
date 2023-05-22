<?php

namespace Database\Factories;

use Illuminate\Database\Eloquent\Factories\Factory;
use Illuminate\Support\Str;

/**
 * @extends \Illuminate\Database\Eloquent\Factories\Factory<\App\Models\User>
 */
class UserFactory extends Factory
{
    /**
     * Define the model's default state.
     *
     * @return array<string, mixed>
     */
    public function definition(): array
    {

        return [
            'id' => Str::uuid(),
            'username' => $username = $this->faker->Name,
            'normalized_username' => Str::lower($username),
            'email' => $email = $this->faker->unique()->safeEmail,
            'normalized_email' => Str::lower($email),
            'email_confirmed' => $this->faker->boolean,
            'password_hash' => bcrypt('123456'),
            'security_stamp' => Str::random(10),
            'concurrency_stamp' => Str::random(10),
            'phone_number' => $this->faker->phoneNumber,
            'phone_number_confirmed' => true,
            'two_factor_enabled' => $this->faker->boolean,
            'lockout_end' => null,
            'lockout_enabled' => $this->faker->boolean,
            'access_failed_count' => $this->faker->randomNumber(),

            'staff_id' => $this->faker->regexify('T[0-9]{4}'),
            'certificate_number' => $this->faker->unique()->randomNumber(6),
            'address' => $this->faker->address,
            'latitude' => $this->faker->latitude(21.5, 25.5, 7),
            'longitude' => $this->faker->longitude(120, 122, 7),
            'radius' => $this->faker->numberBetween(5, 15),
            'remember_token' => Str::random(10),
            'created_at' => now(),
            'updated_at' => now(),
        ];
    }

    /**
     * Indicate that the model's email address should be unverified.
     */
    public function unverified(): static
    {
        return $this->state(fn(array $attributes) => [
            'email_verified_at' => null,
        ]);
    }
}
