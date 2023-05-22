<?php

namespace Database\Factories;

use Illuminate\Database\Eloquent\Factories\Factory;

/**
 * @extends \Illuminate\Database\Eloquent\Factories\Factory<\App\Models\Article>
 */
class ArticleFactory extends Factory
{
    /**
     * Define the model's default state.
     *
     * @return array<string, mixed>
     */
    public function definition(): array
    {
        $users = config('seeding.users');
        $userId = $this->faker->randomElement($users);

        return [
            'user_id' => $userId,
            'title' => $this->faker->sentence,
            'subtitle' => $this->faker->sentence,
            'body' => $this->faker->text,
        ];

    }
}
