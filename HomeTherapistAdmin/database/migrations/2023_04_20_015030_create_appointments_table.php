<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    /**
     * Run the migrations.
     */
    public function up(): void
    {
        Schema::create('appointments', function (Blueprint $table) {
            $table->id();
            $table->string('user_id');
            $table->foreign('user_id')->references('staff_id')->on('users')->cascadeOnUpdate()->restrictOnDelete();
            $table->dateTime('start_dt')->nullable();
            $table->foreign('start_dt')->references('dt')->on('calendar');
            $table->string('customer_ID', 10);
            $table->string('customer_phone', 10);
            $table->string('customer_address');
            $table->decimal('latitude', 10, 7);
            $table->decimal('longitude', 10, 7);
            $table->boolean('is_complete');
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::table('appointments', function (Blueprint $table) {
            $table->dropForeign(['user_id']);
            $table->dropForeign(['start_dt']);
        });
        Schema::dropIfExists('appointments');
    }
};
