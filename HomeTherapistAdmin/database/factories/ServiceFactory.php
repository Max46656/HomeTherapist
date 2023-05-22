<?php

namespace Database\Factories;

use Illuminate\Database\Eloquent\Factories\Factory;

/**
 * @extends \Illuminate\Database\Eloquent\Factories\Factory<\App\Models\Service>
 */
class ServiceFactory extends Factory
{
    /**
     * Define the model's default state.
     *
     * @return array<string, mixed>
     */
    public function definition(): array
    {
        $options = ['環境最佳化', '環境安全與適應性評估', '日常活動訓練', '疼痛管理', '家庭照顧者訓練'];

        return [
            'name' => fake()->unique()->randomElement($options),
            'price' => fake()->numberBetween(2000, 10000),
            'enabled' => true,
        ];
    }
}