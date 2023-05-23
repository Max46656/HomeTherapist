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
        Schema::create('users', function (Blueprint $table) {
            $table->id('id');
            $table->string('staff_id', 50)->index();
            $table->unsignedInteger('certificate_number')->nullable()->unique();
            $table->string('address')->nullable();
            $table->decimal('latitude', 10, 7)->nullable();
            $table->decimal('longitude', 10, 7)->nullable();
            $table->integer('radius')->unsigned()
                ->nullable()->default(12);
            $table->string('username', 256)->nullable();
            $table->string('normalized_username', 256)->nullable();
            $table->string('email', 256)->nullable();
            $table->string('normalized_email', 256)->nullable();
            $table->boolean('email_confirmed')->default(false);
            $table->string('password_hash')->nullable();
            $table->string('password')->nullable();
            $table->string('security_stamp')->nullable();
            $table->string('concurrency_stamp')->nullable();
            $table->string('phone_number')->nullable();
            $table->boolean('phone_number_confirmed')->default(false);
            $table->boolean('two_factor_enabled')->default(false);
            $table->timestamp('lockout_end')->nullable();
            $table->boolean('lockout_enabled')->default(false);
            $table->integer('access_failed_count')->default(0);
            $table->rememberToken();
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     */
    public function down(): void
    {
        Schema::dropIfExists('users');
    }
};
